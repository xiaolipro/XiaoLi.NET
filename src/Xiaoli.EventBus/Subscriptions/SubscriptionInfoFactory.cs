using System;
using System.Collections.Generic;
using System.Text;
using XiaoLi.EventBus.Events;

namespace XiaoLi.EventBus.Subscriptions
{
    public class SubscriptionInfoFactory
    {
        public static SubscriptionInfo CreateDynamicSubscriptionInfo<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler => new SubscriptionInfo(eventName, null, typeof(THandler));

        public static SubscriptionInfo CreateSubscriptionInfo<TEvent, THandler>() where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            string eventName = typeof(TEvent).Name;
            var subscriptionInfo = new SubscriptionInfo(eventName, typeof(TEvent), typeof(THandler));
            return subscriptionInfo;
        }

    }
}
