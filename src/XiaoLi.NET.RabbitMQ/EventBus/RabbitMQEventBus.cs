using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using XiaoLi.NET.EventBus;
using XiaoLi.NET.EventBus.Events;
using XiaoLi.NET.EventBus.Subscriptions;
using XiaoLi.NET.Extensions;
using XiaoLi.NET.RabbitMQ;

namespace XiaoLi.EventBus.RabbitMQ
{
    /// <summary>
    /// 基于RabbitMessageQueue实现的事件总线
    /// 直连交换机，路由模式，以事件名称作为routeKey，一个客户端对应一个队列（以客户端命名）
    /// </summary> 
    public class RabbitMQEventBus : IEventBus
    {
        const string BROKER_NAME = "xiaoli_event_bus";

        private readonly IRabbitMQConnector _rabbitMqConnector;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly int _publishRetries;

        // 客户端订阅队列名称
        private string _subscriptionQueueName;
        // 消费者专用通道，一个客户端一个队列，一个队列一个指定消费者
        private IModel _consumerChannel;

        public RabbitMQEventBus(IRabbitMQConnector rabbitMqConnector,
            ILogger<RabbitMQEventBus> logger,
            IServiceProvider serviceProvider,
            ISubscriptionsManager subscriptionsManager,
            string clientName,
            int retries = 5
        )
        {
            _rabbitMqConnector = rabbitMqConnector;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _subscriptionsManager = subscriptionsManager;
            _subscriptionQueueName = clientName;
            _publishRetries = retries;
            _subscriptionsManager.OnEventRemoved += OnEventRemoved;
            _consumerChannel = CreateConsumerChannel();
        }


        public void Publish(IntegrationEvent @event)
        {
            _rabbitMqConnector.KeepAlive();

            #region 定义重试策略
            var retryPolicy = Policy.Handle<SocketException>() //socket异常时
                .Or<BrokerUnreachableException>() //broker不可达异常时
                .WaitAndRetry(_publishRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(ex, "在{TimeOut}s 后无法连接到RabbitMQ客户端，异常消息：{Message}", $"{time.TotalSeconds:f1}", ex.Message);
                    }
                );
            #endregion

            var eventName = @event.GetType().Name;

            _logger.LogTrace("创建定义RabbitMQ通道以发布事件: {EventId}（{EventName}）", @event.Id, eventName);
            using (var channel = _rabbitMqConnector.CreateChannel())
            {
                _logger.LogTrace("定义RabbitMQ Direct交换机（{ExchangeName}）以发布事件：{EventId}（{EventName}）", BROKER_NAME, @event.Id, eventName);

                channel.ExchangeDeclare(exchange: BROKER_NAME, ExchangeType.Direct);

                var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                retryPolicy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // Non-persistent (1) or persistent (2).

                    _logger.LogInformation("发布事件到RabbitMQ: {EventId}（{EventName}）", @event.Id, eventName);
                    channel.BasicPublish(exchange: BROKER_NAME, routingKey: eventName, mandatory: true,
                        basicProperties: properties, body: body);
                });
            }
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            _logger.LogInformation("{EventHandler}订阅了事件{EventName}", typeof(THandler).GetTypeName(), eventName);
            _subscriptionsManager.AddSubscription<TEvent, THandler>();

            DoRabbitMQSubscription(eventName);
        }


        public void SubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("{EventHandler}订阅了动态事件{EventName}", typeof(THandler).GetTypeName(), eventName);
            _subscriptionsManager.AddDynamicSubscription<THandler>(eventName);

            DoRabbitMQSubscription(eventName);
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            _logger.LogInformation("{EventHandler}取消了对事件{EventName}的订阅", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.RemoveSubscription<TEvent, THandler>();

            DoRabbitMQUnSubscription(eventName);
        }

        public void UnsubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("{EventHandler}取消了对动态事件{EventName}的订阅", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.RemoveDynamicSubscription<THandler>(eventName);

            DoRabbitMQUnSubscription(eventName);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subscriptionQueueName = string.Empty;

            _subscriptionsManager.Clear();
        }

        #region private methods

        /// <summary>
        /// 去RabbitMQ订阅
        /// </summary>
        /// <param name="eventName"></param>
        private void DoRabbitMQSubscription(string eventName)
        {
            if (_subscriptionsManager.HasSubscriptions(eventName)) return;

            _rabbitMqConnector.KeepAlive();

            _consumerChannel.QueueBind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey: eventName);
        }

        /// <summary>
        /// 去RabbitMQ取消订阅
        /// </summary>
        /// <param name="eventName"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DoRabbitMQUnSubscription(string eventName)
        {
            _rabbitMqConnector.KeepAlive();

            _consumerChannel.QueueUnbind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey: eventName);
        }

        private void OnEventRemoved(object sender, string eventName)
        {
            _rabbitMqConnector.KeepAlive();

            using (var channel = _rabbitMqConnector.CreateChannel())
            {
                // 解绑
                channel.QueueUnbind(queue: _subscriptionQueueName, exchange: BROKER_NAME, routingKey: eventName);

                if (_subscriptionsManager.IsEmpty)
                {
                    Dispose();
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
                _logger.LogWarning(ex, "----- 处理消息时出错 \"{Message}\"", message);
            }


            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false); // 手动确认
        }


        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ProcessingBody(string eventName, string message)
        {
            // 空订阅
            if (!_subscriptionsManager.HasSubscriptions(eventName))
            {
                _logger.LogWarning("{EventName}没有任何订阅者", eventName);
                return;
            }

            var subscriptionInfos = _subscriptionsManager.GetSubscriptionInfos(eventName);
            // 广播
            foreach (var subscriptionInfo in subscriptionInfos)
            {
                // 处理动态集成事件
                if (subscriptionInfo.IsDynamic)
                {
                    var handler = _serviceProvider.GetService(subscriptionInfo.HandlerType) as IDynamicIntegrationEventHandler;
                    if (handler == null)
                    {
                        _logger.LogWarning("{EventName}没有实现`{IDynamicIntegrationEventHandler}`", eventName, nameof(IDynamicIntegrationEventHandler));
                        continue;
                    }

                    _logger.LogTrace("正在处理动态集成事件: {EventName}", eventName);

                    await handler.Handle(message);
                }
                else // 处理集成事件
                {
                    var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                    var handler = _serviceProvider.GetService(subscriptionInfo.HandlerType);
                    if (handler == null)
                    {
                        _logger.LogWarning("{EventName}没有实现`IIntegrationEventHandler`", eventName);
                        continue;
                    }

                    var handle = typeof(IIntegrationEventHandler<>)
                        .MakeGenericType(eventType)
                        .GetMethod(nameof(IIntegrationEventHandler<IntegrationEvent>.Handle));

                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                    await Task.Yield();
                    _logger.LogTrace("正在处理集成事件: {EventName}", eventName);
                    handle?.Invoke(handler, new object[] { integrationEvent });
                }
            }
        }

        /// <summary>
        /// 创建消费者通道
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            _rabbitMqConnector.KeepAlive();

            _logger.LogTrace("创建RabbitMQ消费者通道");

            var consumerChannel = _rabbitMqConnector.CreateChannel();

            // 定义直连交换机
            consumerChannel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);

            // 绑定队列
            consumerChannel.QueueDeclare(queue: _subscriptionQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 启动基础消费
            StartBasicConsume();

            // 当通道调用的回调中发生异常时发出信号
            consumerChannel.CallbackException += (sender, args) =>
            {
                _logger.LogWarning(args.Exception, "重新创建RabbitMQ消费者通道");

                // 销毁原有通道，重新创建
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                // 使得新的消费者通道依然能够正常的消费消息
                StartBasicConsume();
            };

            return consumerChannel;
        }

        /// <summary>
        /// 启动基本内容类消费
        /// </summary>
        private void StartBasicConsume()
        {
            if (_consumerChannel == null)
            {
                _logger.LogError("RabbitMQ消费通道_consumerChannel==null，无法启动基本内容类消费");
                return;
            }

            _logger.LogTrace("开启RabbitMQ消费通道的基础消费");

            // 创建异步消费者
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += OnReceived;

            _consumerChannel.BasicConsume(queue: _subscriptionQueueName, autoAck: false, consumer: consumer);
        }
        #endregion
    }
}