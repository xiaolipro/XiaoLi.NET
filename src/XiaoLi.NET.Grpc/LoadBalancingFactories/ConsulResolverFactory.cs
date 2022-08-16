using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories;

public class ConsulResolverFactory: ResolverFactory
{
    private readonly IResolver _resolver;

    public override string Name => _resolver.Name;
    public ConsulResolverFactory(IResolver resolver)
    {
        _resolver = resolver;
    }
    public override Resolver Create(ResolverOptions options)
    {
        return new CustomResolver(options.LoggerFactory, _resolver, options.Address);
    }

    internal class CustomResolver : PollingResolver
    {
        private readonly Uri _address;
        private readonly ILogger _logger;
        private readonly IResolver _resolver;
        private Timer _timer;

        public CustomResolver(ILoggerFactory loggerFactory, IResolver resolver, Uri address) : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(CustomResolver));
            _resolver = resolver;
            _address = address;
        }


        protected override async Task ResolveAsync(CancellationToken cancellationToken)
        {
            // 获取服务对应的所有主机
            var agentServices = await _resolver.ResolutionService(_address.Host);

            var addresses = agentServices.Select(service => new BalancerAddress(service.Address, service.Port)).ToArray();

            // 将结果传递回通道。
            Listener(ResolverResult.ForResult(addresses));
        }
            
        protected override void OnStarted()
        {
            base.OnStarted();

            if (_resolver.RefreshInterval != Timeout.InfiniteTimeSpan)
            {
                _timer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _timer.Change(_resolver.RefreshInterval, _resolver.RefreshInterval);
            }
        }
        private void OnTimerCallback(object state)
        {
            try
            {
                Refresh();
            }
            catch (Exception)
            {
                _logger.LogError("服务解析器刷新失败");
            }
        }
    }

}