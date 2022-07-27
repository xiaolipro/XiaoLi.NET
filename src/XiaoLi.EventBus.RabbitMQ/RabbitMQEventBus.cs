using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using XiaoLi.EventBus.Events;
using XiaoLi.EventBus.Subscriptions;
using XiaoLi.NET.Extensions;
using XiaoLi.Packages.RabbitMQ;

namespace XiaoLi.EventBus.RabbitMQ
{
    /// <summary>
    /// 基于RabbitMessageQueue实现的事件总线
    /// 直连交换机，路由模式，以eventName作为routeKey
    /// </summary> 
    public class RabbitMQEventBus : IEventBus
    {
        const string BROKER_NAME = "xiaoli_event_bus";

        private readonly IRabbitMQConnector _rabbitMqConnector;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly string _subscriptionQueueName;
        private readonly int _publishRetries;
        private readonly IModel _consumerChannel;

        public RabbitMQEventBus(IRabbitMQConnector rabbitMqConnector,
            ILogger<RabbitMQEventBus> logger,
            IServiceProvider serviceProvider,
            ISubscriptionsManager subscriptionsManager,
            string subscriptionQueueName,
            int publishRetries
        )
        {
            _rabbitMqConnector = rabbitMqConnector;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _subscriptionsManager = subscriptionsManager;
            _subscriptionQueueName = subscriptionQueueName;
            _publishRetries = publishRetries;
            _subscriptionsManager.OnEventRemoved += OnEventRemoved;
        }


        public void Publish(IntegrationEvent @event)
        {
            _rabbitMqConnector.KeepAalive();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_publishRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(ex,
                            "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id,
                            $"{time.TotalSeconds:n1}", ex.Message);
                    });
            var eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id,
                eventName);
            using (var channel = _rabbitMqConnector.CreateChannel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
                channel.ExchangeDeclare(exchange: BROKER_NAME, ExchangeType.Direct);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // Non-persistent (1) or persistent (2).

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);
                    channel.BasicPublish(exchange: BROKER_NAME, routingKey: eventName, mandatory: true,
                        basicProperties: properties, body: body);
                });
            }
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName,
                typeof(THandler).GetTypeName());
            _subscriptionsManager.AddSubscription<TEvent, THandler>();

            DoInternalSubscription(eventName);
        }


        public void SubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName,
                typeof(THandler).GetTypeName());
            _subscriptionsManager.AddDynamicSubscription<THandler>(eventName);

            DoInternalSubscription(eventName);
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region private methods

        private void DoInternalSubscription(string eventName)
        {
            if (_subscriptionsManager.HasSubscriptions(eventName)) return;

            _rabbitMqConnector.KeepAalive();

            _consumerChannel.QueueBind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey: eventName);

            _logger.LogTrace("Starting RabbitMQ basic consume");
            // 创建异步消费者
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += OnReceived;

            _consumerChannel.BasicConsume(queue: _subscriptionQueueName, autoAck: false, consumer: consumer);
        }

        private void OnEventRemoved(object sender, string eventName)
        {
            _rabbitMqConnector.KeepAalive();

            using (var channel = _rabbitMqConnector.CreateChannel())
            {
                channel.QueueUnbind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey: eventName);

                if (_subscriptionsManager.IsEmpty)
                {
                    // _subscriptionQueueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        private async Task OnReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            string eventName = eventArgs.RoutingKey;
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            try
            {
                await ProcessingBody(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }


            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false); // 手动确认
        }


        private async Task ProcessingBody(string eventName, string message)
        {
            // 空订阅
            if (!_subscriptionsManager.HasSubscriptions(eventName))
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
                return;
            }

            var subscriptionInfos = _subscriptionsManager.GetSubscriptionInfos(eventName);
            // 广播
            foreach (var subscriptionInfo in subscriptionInfos)
            {
                if (subscriptionInfo.IsDynamic)
                {
                    var handler =
                        _serviceProvider.GetService(subscriptionInfo.HandlerType) as IDynamicIntegrationEventHandler;
                    if (handler == null)
                    {
                        _logger.LogWarning(
                            "No implemented {IDynamicIntegrationEventHandler} for RabbitMQ event: {EventName}",
                            nameof(IDynamicIntegrationEventHandler), eventName);
                        continue;
                    }

                    _logger.LogTrace("Processing RabbitMQ dynamic event: {EventName}", eventName);

                    await handler.Handle(message);
                }
                else
                {
                    var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                    //var handler = _serviceProvider.GetRequiredService(subscriptionInfo.HandlerType) as IIntegrationEventHandler<>;
                    var handler = _serviceProvider.GetService(subscriptionInfo.HandlerType);
                    if (handler == null)
                    {
                        _logger.LogWarning("No implemented IIntegrationEventHandler<> for RabbitMQ event: {EventName}",
                            eventName);
                        continue;
                    }

                    var handle = typeof(IIntegrationEventHandler<>)
                        .MakeGenericType(eventType)
                        .GetMethod(nameof(IIntegrationEventHandler<IntegrationEvent>.Handle));

                    // TODO: how to de-serialize a that not using System.Text.Json.Serializer or Newtonsoft.Json.JsonConvert
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                    await Task.Yield();
                    handle?.Invoke(handler, new object[] { integrationEvent });
                }
            }
        }

        #endregion
    }
}