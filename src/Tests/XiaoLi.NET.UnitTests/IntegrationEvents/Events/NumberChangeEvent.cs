
namespace XiaoLi.NET.UnitTests.IntegrationEvents.Events
{
    public class NumberChangeEvent:Event
    {
        public int Number { get; }

        public NumberChangeEvent(int number)
        {
            Number = number;
        }
    }
}
