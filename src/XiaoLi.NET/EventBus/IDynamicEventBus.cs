using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.EventBus
{
    /// <summary>
    /// 动态事件总线
    /// </summary>
    public interface IDynamicEventBus:IEventBus
    {
        /// <summary>
        /// 订阅动态事件
        /// </summary>
        /// <typeparam name="THandler">动态事件处理者</typeparam>
        /// <param name="eventName">动态事件名称</param>
        void SubscribeDynamic<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler;
        
        /// <summary>
        /// 解阅动态事件
        /// </summary>
        /// <typeparam name="THandler">动态事件处理者</typeparam>
        /// <param name="eventName">动态事件名称</param>
        void UnsubscribeDynamic<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler;
    }
}