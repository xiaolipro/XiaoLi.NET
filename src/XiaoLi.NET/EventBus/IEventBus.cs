using System;
using System.Collections.Generic;
using System.Text;
using XiaoLi.NET.EventBus.Events;

namespace XiaoLi.NET.EventBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event">事件</param>
        void Publish(IntegrationEvent @event);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="TEvent">事件</typeparam>
        /// <typeparam name="THandler">事件处理者</typeparam>
        void Subscribe<TEvent, THandler>() 
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        /// <summary>
        /// 解阅事件
        /// </summary>
        /// <typeparam name="TEvent">事件</typeparam>
        /// <typeparam name="THandler">事件处理者</typeparam>
        void Unsubscribe<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

    }
}
