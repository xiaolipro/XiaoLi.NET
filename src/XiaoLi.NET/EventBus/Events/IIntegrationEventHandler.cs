using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.App.EventBus.Events
{
    /// <summary>
    /// 集成事件处理器
    /// </summary>
    /// <typeparam name="TIntegrationEvent">集成事件</typeparam>
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent:IntegrationEvent
    {
        /// <summary>
        /// 处理集成事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <returns></returns>
        Task Handle(TIntegrationEvent @event);
    }
}
