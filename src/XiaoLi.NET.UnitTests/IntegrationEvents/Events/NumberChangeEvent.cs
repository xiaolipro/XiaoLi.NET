
namespace XiaoLi.NET.UnitTests.IntegrationEvents.Events
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
