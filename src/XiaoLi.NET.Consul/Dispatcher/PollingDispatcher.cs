
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// 轮询调度器
    /// </summary>
    public class PollingDispatcher : AbstractConsulDispatcher
    {
        private static int _counter;
        public PollingDispatcher(ILogger<AbstractConsulDispatcher> logger, ConsulResolver consulResolver) : base(logger, consulResolver) { }

        internal override int GetBalancedIndex(int serviceCount)
        {
            lock (this)
            {
                _counter = _counter == int.MaxValue? 0: ++_counter;
                return _counter % serviceCount;
            }
        }
    }
}
