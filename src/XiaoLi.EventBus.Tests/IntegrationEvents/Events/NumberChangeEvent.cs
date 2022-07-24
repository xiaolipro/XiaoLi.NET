using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoLi.EventBus.Events;

namespace XiaoLi.EventBus.UnitTests.IntegrationEvents.Events
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
