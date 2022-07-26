using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
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
        private readonly IChannelFactory _channelFactory;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;

        public RabbitMQEventBus(IChannelFactory channelFactory, ILogger<RabbitMQEventBus> logger,
            IServiceProvider serviceProvider,
            ISubscriptionsManager subscriptionsManager)
        {
            _channelFactory = channelFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _subscriptionsManager = subscriptionsManager;
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
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

            
            try
            {
                await ProcessingBody(eventName, eventArgs.Body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", Encoding.UTF8.GetString(eventArgs.Body.ToArray()));
            }
        }


        private async Task ProcessingBody(string eventName, ReadOnlyMemory<byte> body)
        {
            if (!_subscriptionsManager.HasSubscriptions(eventName))
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
                return;
            }

            var subscriptionInfos = _subscriptionsManager.GetSubscriptionInfos(eventName);

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
                    string message = Encoding.UTF8.GetString(body.ToArray());
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

                    var handleMethod = typeof(IIntegrationEventHandler<>)
                        .MakeGenericType(eventType)
                        .GetMethod(nameof(IIntegrationEventHandler<IntegrationEvent>.Handle));

                    // TODO: how to de-serialize a that not using System.Text.Json.Serializer or Newtonsoft.Json.JsonConvert
                    var integrationEventInstance = Activator.CreateInstance(eventType);
                    // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                    await Task.Yield();
                    handleMethod?.Invoke(handler, new object[] { integrationEventInstance });
                    
                    var stream = new MemoryStream(body.ToArray());
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(eventType);
                    serializer.ReadObject(stream);
                }
            }
        }
    }
}