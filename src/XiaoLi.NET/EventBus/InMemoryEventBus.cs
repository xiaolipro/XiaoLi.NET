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
    /// <summary>
    /// 基于内存实现的本地事件总线
    /// </summary>
    public class InMemoryEventBus:IEventBus
    {
        private readonly ILogger<InMemoryEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriptionsManager _subscriptionsManager;

        private readonly Channel<Event> _channel; // 事件存储源


        public InMemoryEventBus(ILogger<InMemoryEventBus> logger,
            IServiceProvider serviceProvider,
            ISubscriptionsManager subscriptionsManager,
            IOptions<InMemoryEventBusOptions> eventBusOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _subscriptionsManager = subscriptionsManager;
            var eventBusOptions1 = eventBusOptions.Value;

            var channelOptions = new BoundedChannelOptions(eventBusOptions1.Capacity)
            {
                // channel满了阻塞
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<Event>(channelOptions);
        }

        public void Publish(Event @event)
        {
            _channel.Writer.TryWrite(@event);
        }

        public void Subscribe<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();
            _logger.LogInformation("{EventHandler}订阅了事件{EventName}", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.AddSubscription<TEvent, THandler>();

            StartBasicConsume();
        }

        private void StartBasicConsume()
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            taskFactory.StartNew(async () =>
            {
                while (true)
                {
                    // Consumer patterns
                    // May throw ChannelClosedException if the parent channel's writer signals complete.
                    // Note. This code will throw an exception if the channel is closed.
                    // Details see: https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
                    var @event = await _channel.Reader.ReadAsync();
                    
                    await Processing(@event);
                }
            });
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : Event where THandler : IEventHandler<TEvent>
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            _logger.LogInformation("{EventHandler}解除了对事件{EventName}的订阅", typeof(THandler).GetTypeName(), eventName);

            _subscriptionsManager.RemoveSubscription<TEvent, THandler>();
        }

        private async Task Processing(Event @event)
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
                    _logger.LogWarning("{EventHandlerName}没有实现`IIntegrationEventHandler`", nameof(handler));
                    continue;
                }

                var handle = typeof(IEventHandler<>)
                    .MakeGenericType(eventType)
                    .GetMethod(nameof(IEventHandler<Event>.Handle));

                // see：https://stackoverflow.com/questions/22645024/when-would-i-use-task-yield
                await Task.Yield();
                _logger.LogTrace("正在处理集成事件: {EventName}", eventName);
                handle?.Invoke(handler, new object[] { @event });
            }
        }
    }
}
#endif