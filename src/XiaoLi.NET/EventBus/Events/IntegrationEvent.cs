using System;

namespace XiaoLi.NET.EventBus.Events
{
    /// <summary>
    /// 集成事件
    /// </summary>
    public class IntegrationEvent
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; }
        
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
        }
    }
}
