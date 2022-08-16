using System.Collections.Generic;
using Grpc.Net.Client.Balancer;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories;

public class ConsulBalancerFactory:LoadBalancerFactory
{
    private readonly IBalancer _balancer;
    public override string Name => _balancer.Name;
    
    public ConsulBalancerFactory(IBalancer balancer)
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
            return new CustomPicker(readySubchannels, _balancer,_logger);
        }

        private class CustomPicker : SubchannelPicker
        {
            private readonly IReadOnlyList<Subchannel> _subchannels;
            private readonly IBalancer _balancer;
            private readonly ILogger _logger;

            public CustomPicker(IReadOnlyList<Subchannel> subchannels, IBalancer balancer,ILogger logger)
            {
                _subchannels = subchannels;
                _balancer = balancer;
                _logger = logger;
            }

            public override PickResult Pick(PickContext context)
            {
                int index = _balancer.Pick(_subchannels.Count);
                var channel = _subchannels[index];
                
                _logger.LogInformation($"来自{_balancer.Name}均衡器的选取结果：{channel.CurrentAddress}");
                // Pick a random subchannel.
                return PickResult.ForSubchannel(channel);
            }
        }
    }
}