using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoLi.EventBus.Subscriptions
{
    public class SubscriptionInfo
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public Type EvenType { get;}

        /// <summary>
        /// 事件处理者类型
        /// </summary>
        public Type HandlerType { get; }

        public SubscriptionInfo(string eventName, Type eventType, Type handlerType)
        {
            EventName = eventName;
            EvenType = eventType;
            HandlerType = handlerType;
        }
    }
}
