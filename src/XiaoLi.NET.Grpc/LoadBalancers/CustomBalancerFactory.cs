using System;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using XiaoLi.NET.LoadBalancers;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public class CustomBalancerFactory : LoadBalancerFactory
    {
        private readonly ILoadBalancer _loadBalancer;

        public override string Name => string.IsNullOrWhiteSpace(_loadBalancer.Name)?"grpc":_loadBalancer.Name;
        public CustomBalancerFactory(ILoadBalancer loadBalancer)
        {
            _loadBalancer = loadBalancer;
        }
        public override LoadBalancer Create(LoadBalancerOptions options)
        {
            return new CustomBalancer(options.Controller, options.LoggerFactory, _loadBalancer);
        }
        
       


        internal class CustomBalancer : SubchannelsLoadBalancer
        {
            private readonly ILoadBalancer _loadBalancer;

            public CustomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory, ILoadBalancer loadBalancer)
                : base(controller, loggerFactory)
            {
                _loadBalancer = loadBalancer;
            }


            protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
            {
                return new CustomPicker(readySubchannels, _loadBalancer);
            }

            private class CustomPicker : SubchannelPicker
            {
                private readonly IReadOnlyList<Subchannel> _subchannels;
                private readonly ILoadBalancer _loadBalancer;

                public CustomPicker(IReadOnlyList<Subchannel> subchannels, ILoadBalancer loadBalancer)
                {
                    _subchannels = subchannels;
                    _loadBalancer = loadBalancer;
                }

                public override PickResult Pick(PickContext context)
                {
                    int index = _loadBalancer.GetBalancedIndex(_subchannels.Count);
                    // Pick a random subchannel.
                    return PickResult.ForSubchannel(_subchannels[index]);
                }
            }
        }

    }
}
