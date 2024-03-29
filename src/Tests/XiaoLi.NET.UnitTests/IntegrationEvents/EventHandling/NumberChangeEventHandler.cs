﻿


namespace XiaoLi.NET.UnitTests.IntegrationEvents.EventHandling
{
    public class NumberChangeEventHandler:IEventHandler<NumberChangeEvent>
    {
        public int Number { get; private set; }
        public Task Handle(NumberChangeEvent @event)
        {
            Number = @event.Number;

            return Task.CompletedTask;
        }
    }
}
