using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancers;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public class CustomResolverFactory : ResolverFactory
    {
        private readonly ILoadBalancer _loadBalancer;

        // Create a FileResolver when the URI has a 'grpc' scheme.
        public override string Name => string.IsNullOrWhiteSpace(_loadBalancer.Name) ? "grpc" : _loadBalancer.Name;

        public CustomResolverFactory(ILoadBalancer loadBalancer)
        {
            _loadBalancer = loadBalancer;
        }

        public override Resolver Create(ResolverOptions options)
        {
            return new CustomResolver(options.LoggerFactory, _loadBalancer, options.Address);
        }

        internal class CustomResolver : PollingResolver
        {
            private readonly Uri _address;
            private readonly ILogger _logger;
            private readonly ILoadBalancer _loadBalancer;
            private Timer _timer;

            public CustomResolver(ILoggerFactory loggerFactory, ILoadBalancer loadBalancer, Uri address) : base(loggerFactory)
            {
                _logger = loggerFactory.CreateLogger(typeof(CustomResolver));
                _loadBalancer = loadBalancer;
                _address = address;
            }


            protected override async Task ResolveAsync(CancellationToken cancellationToken)
            {
                // 获取服务对应的所有主机
                var uris = await _loadBalancer.ResolutionService(_address.Host);

                var addresses = uris.Select(uri => new BalancerAddress(uri.Host, uri.Port)).ToArray();

                // 将结果传递回通道。
                Listener(ResolverResult.ForResult(addresses));
            }
            
            protected override void OnStarted()
            {
                base.OnStarted();

                if (_loadBalancer.RefreshInterval != Timeout.InfiniteTimeSpan)
                {
                    _timer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                    _timer.Change(_loadBalancer.RefreshInterval, _loadBalancer.RefreshInterval);
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
}