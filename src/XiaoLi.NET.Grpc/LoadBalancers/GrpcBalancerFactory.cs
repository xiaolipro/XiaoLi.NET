using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public class GrpcBalancerFactory : LoadBalancerFactory
    {
        private readonly IGrpcLoadBalancer _grpcLoadBalancer;

        public override string Name => _grpcLoadBalancer.Name;
        public GrpcBalancerFactory(IGrpcLoadBalancer grpcLoadBalancer)
        {
            _grpcLoadBalancer = grpcLoadBalancer;
        }
        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new Balancer(options.Controller, options.LoggerFactory, _grpcLoadBalancer);
        }


        internal class Balancer : SubchannelsLoadBalancer
        {
            private readonly IGrpcLoadBalancer _grpcLoadBalancer;

            public Balancer(IChannelControlHelper controller, ILoggerFactory loggerFactory, IGrpcLoadBalancer grpcLoadBalancer)
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
                    int index = _grpcLoadBalancer.GetIndex();
                    // Pick a random subchannel.
                    return PickResult.ForSubchannel(_subchannels[index]);
                }
            }
        }

    }
}
