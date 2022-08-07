using System;

namespace XiaoLi.NET.App.EventBus.Events
{
    /// <summary>
    /// 事件接口
    /// </summary>
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 事件唯一标识
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 事件创建时间
        /// </summary>
        public DateTime CreationTime { get; }
    }
}
