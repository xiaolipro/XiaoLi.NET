using System;

namespace XiaoLi.EventBus.Events
{
    /// <summary>
    /// 集成事件
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
