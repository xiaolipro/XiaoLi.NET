using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.EventBus.Events
{
    /// <summary>
    /// 强类型事件处理器
    /// </summary>
    /// <typeparam name="TEvent">事件</typeparam>
    public interface IEventHandler<in TEvent> where TEvent:Event
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event">事件携带的数据</param>
        /// <returns></returns>
        Task Handle(TEvent @event);
    }
}
