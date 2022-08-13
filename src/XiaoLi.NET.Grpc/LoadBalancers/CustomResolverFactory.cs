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
        private readonly IGrpcLoadBalancer _grpcLoadBalancer;

        // Create a FileResolver when the URI has a 'grpc' scheme.
        public override string Name => string.IsNullOrWhiteSpace(_grpcLoadBalancer.Name) ? "grpc" : _grpcLoadBalancer.Name;

        public CustomResolverFactory(IGrpcLoadBalancer grpcLoadBalancer)
        {
            _grpcLoadBalancer = grpcLoadBalancer;
        }

        public override Resolver Create(ResolverOptions options)
        {
            return new CustomResolver(options.LoggerFactory, _grpcLoadBalancer, options.Address, options.DefaultPort);
        }

        internal class CustomResolver : PollingResolver
        {
            private readonly Uri _address;
            private readonly int _defaultPort;
            private readonly IGrpcLoadBalancer _grpcLoadBalancer;

            public CustomResolver(ILoggerFactory loggerFactory, IGrpcLoadBalancer grpcLoadBalancer, Uri address,
                int defaultPort) : base(loggerFactory)
            {
                _grpcLoadBalancer = grpcLoadBalancer;
                _address = address;
                _defaultPort = defaultPort;
            }


            protected override async Task ResolveAsync(CancellationToken cancellationToken)
            {
                // 获取服务对应的所有主机
                var uris = await _grpcLoadBalancer.GetServiceUris(_address.Host);

                var addresses = uris.Select(uri => new BalancerAddress(uri.Host, uri.Port)).ToArray();

                // 将结果传递回通道。
                Listener(ResolverResult.ForResult(addresses));
            }
        }
    }
}