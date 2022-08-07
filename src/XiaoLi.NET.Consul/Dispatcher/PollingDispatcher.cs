
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.App.Consul.Dispatcher
{
    /// <summary>
    /// 轮询调度器
    /// </summary>
    public class PollingDispatcher : AbstractConsulDispatcher
    {
        private static int _counter;
        public PollingDispatcher(ILogger<AbstractConsulDispatcher> logger, IOptionsMonitor<ConsulClientOptions> options) : base(logger, options) { }

        protected override int GetAgentServiceIndex()
        {
            lock (this)
            {
                _counter = _counter == int.MaxValue? 0: _counter++;
                return _counter % AgentServices.Count;
            }
        }
    }
}
