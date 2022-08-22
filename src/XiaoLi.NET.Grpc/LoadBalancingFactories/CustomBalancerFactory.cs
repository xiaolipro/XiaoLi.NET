using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories;

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
        private readonly ILogger _logger;
        private readonly IBalancer _balancer;

        public CustomBalancer(IChannelControlHelper controller, ILoggerFactory loggerFactory, IBalancer balancer)
            : base(controller, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(CustomBalancer));
            _balancer = balancer;
        }


        protected override SubchannelPicker CreatePicker(IReadOnlyList<Subchannel> readySubchannels)
        {
            return new CustomPicker(readySubchannels, _balancer, _logger);
        }

        private class CustomPicker : SubchannelPicker
        {
            private readonly IReadOnlyList<Subchannel> _subchannels;
            private readonly IBalancer _balancer;
            private readonly ILogger _logger;

            public CustomPicker(IReadOnlyList<Subchannel> subchannels, IBalancer balancer, ILogger logger)
            {
                _subchannels = subchannels;
                _balancer = balancer;
                _logger = logger;
            }

            public override PickResult Pick(PickContext context)
            {
                int index = _balancer.Pick(_subchannels.Count);
                var channel = _subchannels[index];

                _logger.LogInformation("来自{BalancerName}均衡器{Count}选1的结果：{ChannelCurrentAddress}", 
                    _balancer.Name, _subchannels.Count, channel.CurrentAddress);
                // Pick a sub-channel.
                var res= PickResult.ForSubchannel(channel);
                
                //_logger.LogInformation("Status----{Status}",res.Status.ToString());

                return res;
            }
        }
    }
}