using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XiaoLi.EventBus.Events;
using XiaoLi.EventBus.Subscriptions;
using XiaoLi.Packages.RabbitMQ;

namespace XiaoLi.EventBus.RabbitMQ
{
    /// <summary>
    /// 基于RabbitMessageQueue实现的事件总线
    /// </summary>
    public class RabbitMQEventBus:IEventBus
    {
        private readonly IChannelFactory _channelFactory;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;

        public RabbitMQEventBus(IChannelFactory channelFactory,ILogger<RabbitMQEventBus> logger, IServiceProvider serviceProvider,
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

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Subscribe<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
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


        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);
            if (!_subscriptionsManager.HasSubscriptions(eventName))
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
                return;
            }

            var subscriptionInfos = _subscriptionsManager.GetSubscriptionInfos(eventName);

            foreach (var subscriptionInfo in subscriptionInfos)
            {
                if (subscriptionInfo.EvenType)
            }
        }
    }
}