using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories;

public class CustomResolverFactory: ResolverFactory
{
    private readonly IResolver _resolver;

    public override string Name => _resolver.Name;
    public CustomResolverFactory(IResolver resolver)
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
            if (string.IsNullOrWhiteSpace(address.Host)) throw new ArgumentNullException(nameof(address));
            _address = address;
        }


        protected override async Task ResolveAsync(CancellationToken cancellationToken)
        {
            // 获取服务对应的所有主机
            var (uris, metaData) = await _resolver.ResolutionService(_address.Host);
            
            // 空的balancer-address会引发internal崩溃
            if (uris == null || uris.Count < 1) return;
            
            var addresses = uris.Select(uri => new BalancerAddress(uri.Host, uri.Port)).ToArray();

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
                _logger.LogInformation("重新解析服务", _resolver.RefreshInterval.TotalSeconds);
                Refresh();
            }
            catch (Exception)
            {
                _logger.LogError("服务解析器刷新失败");
            }
        }
    }

}