using Microsoft.Extensions.Logging;
using XiaoLi.EventBus.Events;
using XiaoLi.RabbitMQ;

namespace XiaoLi.EventBus.RabbitMQ
{
    /// <summary>
    /// 基于RabbitMessageQueue实现的事件总线
    /// </summary>
    public class RabbitMQEventBus:IEventBus
    {
        private readonly IRabbitMQConnector _connector;
        private readonly ILogger<RabbitMQEventBus> _logger;

        public RabbitMQEventBus(IRabbitMQConnector connector,ILogger<RabbitMQEventBus> logger,)
        {
            _connector = connector;
            _logger = logger;
        }
        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Subscribe<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}