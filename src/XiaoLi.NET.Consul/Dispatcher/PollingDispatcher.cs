
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// 轮询调度器
    /// </summary>
    public class PollingDispatcher : AbstractConsulDispatcher
    {
        private static int _counter;
        public PollingDispatcher(ILogger<AbstractConsulDispatcher> logger, IOptions<ConsulClientOptions> options) : base(logger, options) { }

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
