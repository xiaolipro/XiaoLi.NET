using System;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories
{
    public class CustomBalancerFactory : LoadBalancerFactory
    {
        private readonly IBalancer _balancer;

        public override string Name => _balancer.Name;

        public CustomBalancerFactory(IBalancer balancer)
        {
            _balancer = balancer;
        }

        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new CustomBalancer(options.Controller, options.LoggerFactory, _balancer);
        }


        internal class CustomBalancer : SubchannelsLoadBalancer
        {
            private readonly IBalancer _balancer;

            public CustomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory, IBalancer balancer)
                : base(controller, loggerFactory)
            {
                _balancer = balancer;
            }


            protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
            {
                return new CustomPicker(readySubchannels, _balancer);
            }

            private class CustomPicker : SubchannelPicker
            {
                private readonly IReadOnlyList<Subchannel> _subchannels;
                private readonly IBalancer _balancer;

                public CustomPicker(IReadOnlyList<Subchannel> subchannels, IBalancer balancer)
                {
                    _subchannels = subchannels;
                    _balancer = balancer;
                }

                public override PickResult Pick(PickContext context)
                {
                    int index = _balancer.Pick(_subchannels.Count);
                    // Pick a random subchannel.
                    return PickResult.ForSubchannel(_subchannels[index]);
                }
            }
        }
    }
}