
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// 平均调度器
    /// </summary>
    public class AverageDispatcher:AbstractConsulDispatcher
    {
        private static int _seed;
        public AverageDispatcher(ILogger<AbstractConsulDispatcher> logger, ConsulResolver consulResolver) : base(logger, consulResolver) { }

        internal override int GetBalancedIndex(int serviceCount)
        {
            lock (this)
            {
                _seed = _seed == int.MaxValue ? 0 : ++_seed;
                return new Random(_seed).Next(0, serviceCount);
            }
        }
    }
}
