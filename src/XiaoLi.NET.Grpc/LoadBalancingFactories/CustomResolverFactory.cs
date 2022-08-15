using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories
{
    public class CustomResolverFactory : ResolverFactory
    {
        private readonly IGrpcResolver _grpcResolver;

        // Create a FileResolver when the URI has a 'grpc' scheme.
        public override string Name => _grpcResolver.Name ;

        public CustomResolverFactory(IGrpcResolver grpcResolver)
        {
            _grpcResolver = grpcResolver;
        }

        public override Resolver Create(ResolverOptions options)
        {
            return new CustomResolver(options.LoggerFactory, _grpcResolver, options.Address);
        }

        internal class CustomResolver : PollingResolver
        {
            private readonly Uri _address;
            private readonly ILogger _logger;
            private readonly IGrpcResolver _balancer;
            private Timer _timer;

            public CustomResolver(ILoggerFactory loggerFactory, IGrpcResolver balancer, Uri address) : base(loggerFactory)
            {
                _logger = loggerFactory.CreateLogger(typeof(CustomResolver));
                _balancer = balancer;
                _address = address;
            }


            protected override async Task ResolveAsync(CancellationToken cancellationToken)
            {
                // 获取服务对应的所有主机
                var uris = await _balancer.ResolutionGrpcService(_address.Host);

                var addresses = uris.Select(uri => new BalancerAddress(uri.Host, uri.Port)).ToArray();

                // 将结果传递回通道。
                Listener(ResolverResult.ForResult(addresses));
            }
            
            protected override void OnStarted()
            {
                base.OnStarted();

                if (_balancer.RefreshInterval != Timeout.InfiniteTimeSpan)
                {
                    _timer = new Timer(OnTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                    _timer.Change(_balancer.RefreshInterval, _balancer.RefreshInterval);
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