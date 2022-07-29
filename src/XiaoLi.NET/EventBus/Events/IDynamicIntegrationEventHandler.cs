using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.EventBus.Events
{
    /// <summary>
    /// 动态集成事件处理器
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        /// <summary>
        /// 处理动态事件
        /// </summary>
        /// <param name="message">队列中的消息</param>
        /// <returns></returns>
        Task Handle(string message);
    }
}
