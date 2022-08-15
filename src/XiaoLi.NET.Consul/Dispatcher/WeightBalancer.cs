using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    public class WeightBalancer : IBalancer
    {
        private static int _seed;
        public string Name { get; } = nameof(WeightBalancer);

        public int Pick(List<dynamic> services)
        {
            var targets = new List<int>();

            for (int i = 0; i < services.Count; i++)
            {
                int.TryParse(services[i].Meta["Weight"], out int count);
                targets.AddRange(Enumerable.Repeat(i, count));
            }

            if (_seed > 0x3ffffff) _seed = 0;
            var number = new Random(_seed++).Next(0, int.MaxValue) % targets.Count;
            return targets[number];
        }
    }
}