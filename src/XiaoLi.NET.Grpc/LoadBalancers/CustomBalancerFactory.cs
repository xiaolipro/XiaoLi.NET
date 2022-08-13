using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using XiaoLi.NET.LoadBalancers;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public class CustomBalancerFactory : LoadBalancerFactory
    {
        private readonly IGrpcLoadBalancer _grpcLoadBalancer;

        public override string Name => string.IsNullOrWhiteSpace(_grpcLoadBalancer.Name)?"grpc":_grpcLoadBalancer.Name;
        public CustomBalancerFactory(IGrpcLoadBalancer grpcLoadBalancer)
        {
            _grpcLoadBalancer = grpcLoadBalancer;
        }
        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new CustomBalancer(options.Controller, options.LoggerFactory, _grpcLoadBalancer);
        }


        internal class CustomBalancer : SubchannelsLoadBalancer
        {
            private readonly IGrpcLoadBalancer _grpcLoadBalancer;

            public CustomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory, IGrpcLoadBalancer grpcLoadBalancer)
                : base(controller, loggerFactory)
            {
                _grpcLoadBalancer = grpcLoadBalancer;
            }


            protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
            {
                return new ConsulPicker(readySubchannels, _grpcLoadBalancer);
            }

            private class ConsulPicker : SubchannelPicker
            {
                private readonly IReadOnlyList<Subchannel> _subchannels;
                private readonly IGrpcLoadBalancer _grpcLoadBalancer;

                public ConsulPicker(IReadOnlyList<Subchannel> subchannels, IGrpcLoadBalancer grpcLoadBalancer)
                {
                    _subchannels = subchannels;
                    _grpcLoadBalancer = grpcLoadBalancer;
                }

                public override PickResult Pick(PickContext context)
                {
                    int index = _grpcLoadBalancer.GetBalancedIndex(_subchannels.Count);
                    // Pick a random subchannel.
                    return PickResult.ForSubchannel(_subchannels[index]);
                }
            }
        }

    }
}
