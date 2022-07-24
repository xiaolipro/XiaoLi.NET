using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.EventBus.Events
{
    /// <summary>
    /// 动态集成事件处理器
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        /// <summary>
        /// 处理动态事件
        /// </summary>
        /// <param name="event">动态事件</param>
        /// <returns></returns>
        Task Handle(dynamic @event);
    }
}
