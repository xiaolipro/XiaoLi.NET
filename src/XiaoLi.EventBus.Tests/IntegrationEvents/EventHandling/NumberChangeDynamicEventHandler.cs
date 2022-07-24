using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.EventBus.UnitTests.IntegrationEvents.EventHandling
{
    public class NumberChangeDynamicEventHandler : IDynamicIntegrationEventHandler
    {
        public int Number { get; private set; }
        public Task Handle(dynamic @event)
        {
            Number = @event.Number + 1;

            return Task.CompletedTask;
        }
    }
}
