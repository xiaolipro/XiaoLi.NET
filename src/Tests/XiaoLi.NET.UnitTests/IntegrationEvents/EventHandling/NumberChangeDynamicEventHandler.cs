namespace XiaoLi.NET.UnitTests.IntegrationEvents.EventHandling
{
    public class NumberChangeDynamicEventHandler : IDynamicIntegrationEventHandler
    {
        public Task Handle(string message)
        {
            return Task.CompletedTask;
        }
    }
}
