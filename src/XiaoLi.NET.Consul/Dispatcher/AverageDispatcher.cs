
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// 平均调度器
    /// </summary>
    public class AverageDispatcher:AbstractConsulDispatcher
    {
        private static int _seed;
        public AverageDispatcher(ILogger<AbstractConsulDispatcher> logger, IOptionsMonitor<ConsulClientOptions> options) : base(logger, options) { }

        protected override int GetAgentServiceIndex()
        {
            lock (this)
            {
                _seed = _seed == int.MaxValue ? 0 : _seed++;
                return new Random(_seed).Next(0, AgentServices.Count);
            }
        }
    }
}
