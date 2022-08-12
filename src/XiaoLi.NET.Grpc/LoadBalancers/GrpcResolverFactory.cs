using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public class GrpcResolverFactory : ResolverFactory
    {
        private readonly IGrpcLoadBalancer _grpcLoadBalancer;

        // Create a FileResolver when the URI has a 'consul' scheme.
        public override string Name => "consul";

        public GrpcResolverFactory(IGrpcLoadBalancer grpcLoadBalancer)
        {
            _grpcLoadBalancer = grpcLoadBalancer;
        }

        public override Resolver Create(ResolverOptions options)
        {
            return new ConsulResolver(options.LoggerFactory, _grpcLoadBalancer, options.Address, options.DefaultPort);
        }

        internal class ConsulResolver : PollingResolver
        {
            private readonly Uri _address;
            private readonly int _defaultPort;
            private readonly IGrpcLoadBalancer _grpcLoadBalancer;

            public ConsulResolver(ILoggerFactory loggerFactory, IGrpcLoadBalancer grpcLoadBalancer, Uri address, int defaultPort) : base(loggerFactory)
            {
                _grpcLoadBalancer = grpcLoadBalancer;
                _address = address;
                _defaultPort = defaultPort;
            }


            protected override Task ResolveAsync(CancellationToken cancellationToken)
            {
                // 依赖Consul的服务发现，获取服务对应的所有主机
                var uris = _grpcLoadBalancer.GetServiceUris(_address.Host);

                var addresses = uris.Select(uri => new BalancerAddress(uri.Host, _defaultPort)).ToArray();

                // 将结果传递回通道。
                Listener(ResolverResult.ForResult(addresses));

                return Task.CompletedTask;
            }
        }
    }
}
