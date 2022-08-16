using System;
using System.Collections.Generic;
using System.Linq;
using XiaoLi.NET.Extensions;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    public class WeightBalancer : IBalancer
    {
        private static int _seed;
        public string Name { get; } = nameof(WeightBalancer);

        public int Pick(int serviceCount, dynamic metaData = default)
        {
            var weights = metaData as IEnumerable<int> ?? throw new ArgumentNullException(nameof(metaData));
            var targets = new List<int>();

            foreach (var (weight,idx) in weights.WithIndex())
            {
                targets.AddRange(Enumerable.Repeat(idx, weight));
            }

            if (_seed > 0x3ffffff) _seed = 0;
            var number = new Random(_seed++).Next(0, int.MaxValue) % targets.Count;
            return targets[number];
        }
    }
}