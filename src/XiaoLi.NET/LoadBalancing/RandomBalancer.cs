using System;
using System.Collections.Generic;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.LoadBalancing
{
    /// <summary>
    /// 随机调度器
    /// </summary>
    public class RandomBalancer : IBalancer
    {
        private static int _seed;
        public string Name { get; } = nameof(RandomBalancer);

        public int Pick(List<dynamic> services)
        {
            lock (this)
            {
                if (_seed > 0x3fffffff) _seed = 0;
                return new Random(_seed ++).Next(0, services.Count);
            }
        }
    }
}