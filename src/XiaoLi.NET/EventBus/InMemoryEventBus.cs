#if NETCOREAPP
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.EventBus.Events;
using XiaoLi.NET.EventBus.Subscriptions;
using XiaoLi.NET.Extensions;

namespace XiaoLi.NET.EventBus
{
    public class InMemoryEventBusOptions
    {
        
    }
    public class InMemoryEventBus:IEventBus
    {
        private readonly ILogger<InMemoryEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;
        private readonly IOptions<InMemoryEventBusOptions> _eventBusOptions;
        
        private readonly Channel<IntegrationEvent> _channel; // 事件存储源


        public InMemoryEventBus(ILogger<InMemoryEventBus> logger,
            IServiceProvider serviceProvider,
            ISubscriptionsManager subscriptionsManager,
            IOptions<InMemoryEventBusOptions> eventBusOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _subscriptionsManager = subscriptionsManager;
            _eventBusOptions = eventBusOptions;
        }

        public void Publish(IntegrationEvent @event)
        {
            _channel.Writer.TryWrite(@event);
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();
            _logger.LogInformation("{EventHandler}订阅了事件{EventName}", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.AddSubscription<TEvent, THandler>();

            StartBasicConsume();
        }

        private void StartBasicConsume()
        {
            var taskFacotry = new TaskFactory(TaskScheduler.Current);
            taskFacotry.StartNew(async () =>
            {
                while (true)
                {
                    // Consumer patterns
                    // May throw ChannelClosedException if the parent channel's writer signals complete.
                    // Note. This code will throw an exception if the channel is closed.
                    var @event = await _channel.Reader.ReadAsync();
                    
                    await Processing(@event);
                }
            });
        }

        public void SubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            _logger.LogInformation("{EventHandler}解除了对事件{EventName}的订阅", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.RemoveSubscription<TEvent, THandler>();
        }

        public void UnsubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }


        private async Task Processing(IntegrationEvent @event)
        {
            string eventName = @event.GetType().GetTypeName();
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

                // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                await Task.Yield();
                _logger.LogTrace("正在处理集成事件: {EventName}", eventName);
                handle?.Invoke(handler, new object[] { @event });
            }
        }
    }
}
#endif