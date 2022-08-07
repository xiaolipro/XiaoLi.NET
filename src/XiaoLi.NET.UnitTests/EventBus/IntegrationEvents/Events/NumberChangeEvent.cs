
namespace XiaoLi.NET.App.UnitTests.EventBus.IntegrationEvents.Events
{
    public class NumberChangeEvent:IntegrationEvent
    {
        public int Number { get; }

        public NumberChangeEvent(int number)
        {
            Number = number;
        }
    }
}
