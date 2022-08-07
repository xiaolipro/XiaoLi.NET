
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.App.Consul.Dispatcher
{
    public class WeightDispatcher : AbstractConsulDispatcher
    {
        private static int _seed;
        public WeightDispatcher(ILogger<AbstractConsulDispatcher> logger, IOptionsMonitor<ConsulClientOptions> options) : base(logger, options)
        {
        }

        protected override int GetAgentServiceIndex()
        {
            var targets = new List<int>();

            for (int i = 0; i < AgentServices.Count; i++)
            {
                int.TryParse(AgentServices[i].Meta["Weight"], out int count);
                targets.AddRange(Enumerable.Repeat(i, count));
            }

            _seed = _seed > 0x3ffffff ? 0 : _seed++;  // 未处理线程安全问题，大并发下可能获得相同的种子
            var number = new Random(_seed).Next(0, int.MaxValue) % targets.Count;
            return targets[number];
        }
    }
}
