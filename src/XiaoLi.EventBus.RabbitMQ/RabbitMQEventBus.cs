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
using XiaoLi.Packages.RabbitMQ;

namespace XiaoLi.EventBus.RabbitMQ
{
    /// <summary>
    /// 基于RabbitMessageQueue实现的事件总线
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

        private void OnEventRemoved(object sender, string eventName)
        {
            _rabbitMqConnector.KeepAalive();

            using (var channel = _rabbitMqConnector.CreateChannel())
            {
                channel.QueueUnbind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey:eventName);

                if (_subscriptionsManager.IsEmpty)
                {
                    _consumerChannel.Close();
                }
            }
        }

        public void Publish(IntegrationEvent @event)
        {
            _rabbitMqConnector.KeepAalive();
            
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry()
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            DoSubscribe(eventName);
        }

        private void DoSubscribe(string eventName)
        {
            if(_subscriptionsManager.HasSubscriptions(eventName)) return;

            if (!_rabbitMqConnector.IsConnected)
            {
                _rabbitMqConnector.ReConnect();
            }
            
            _consumerChannel.QueueBind(_subscriptionOptions.QueueName);
        }

        public void Subscribe<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs)
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
                    var integrationEvent = JsonConvert.DeserializeObject(message,eventType); 
                    // Activator.CreateInstance(eventType);
                    // var stream = new MemoryStream(body.ToArray());
                    // DataContractJsonSerializer serializer = new DataContractJsonSerializer(eventType);
                    // serializer.ReadObject(stream);
                    
                    // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                    await Task.Yield();
                    handle?.Invoke(handler, new object[] { integrationEvent });
                }
            }
        }
    }
}