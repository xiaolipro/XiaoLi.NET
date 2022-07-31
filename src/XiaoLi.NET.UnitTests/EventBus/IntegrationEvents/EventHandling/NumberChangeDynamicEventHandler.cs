namespace XiaoLi.NET.UnitTests.EventBus.IntegrationEvents.EventHandling
{
    public class NumberChangeDynamicEventHandler : IDynamicIntegrationEventHandler
    {
        public Task Handle(string message)
        {
            return Task.CompletedTask;
        }
    }
}
