using Microsoft.Extensions.Logging;
using XiaoLi.NET.Consul.Dispatcher;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.LoadBalancing
{
    public class ConsulBalancer: IBalancer
    {
        private readonly ILogger<ConsulBalancer> _logger;
        private readonly AbstractConsulDispatcher _abstractConsulDispatcher;

        public ConsulBalancer(ILogger<ConsulBalancer> logger, AbstractConsulDispatcher abstractConsulDispatcher) 
        {
            _logger = logger;
            _abstractConsulDispatcher = abstractConsulDispatcher;
        }

        public string Name { get; } = nameof(ConsulBalancer);

        public int Pick(int serviceCount)
        {
            return _abstractConsulDispatcher.GetBalancedIndex(serviceCount);
        }

    }
}