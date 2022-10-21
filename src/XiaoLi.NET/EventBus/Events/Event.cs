using System;

namespace XiaoLi.NET.EventBus.Events
{
    /// <summary>
    /// 集成事件
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; }
        
        public Event()
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
        }
    }
}