namespace XiaoLi.NET.UnitTests.IntegrationEvents.EventHandling
{
    public class NumberChangeDynamicEventHandler : IDynamicEventHandler
    {
        public Task Handle(string message)
        {
            return Task.CompletedTask;
        }
    }
}
