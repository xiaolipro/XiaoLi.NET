using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.Dispatcher;
using XiaoLi.NET.LoadBalancers;

namespace XiaoLi.NET.Consul.Grpc
{
    public class ConsulGrpcLoadBalancer: IGrpcLoadBalancer
    {
        private readonly ILogger<AbstractConsulDispatcher> _logger;
        private readonly AbstractConsulDispatcher _abstractConsulDispatcher;
        private readonly ConsulClientOptions _consulClientOptions;

        public ConsulGrpcLoadBalancer(ILogger<AbstractConsulDispatcher> logger, AbstractConsulDispatcher abstractConsulDispatcher,IOptions<ConsulClientOptions> options) 
        {
            _logger = logger;
            _abstractConsulDispatcher = abstractConsulDispatcher;
            _consulClientOptions = options.Value;
        }

        public string Name { get; } = "consul";

        public int GetBalancedIndex(int serviceCount)
        {
            return _abstractConsulDispatcher.GetBalancedIndex(serviceCount);
        }

        public async Task<List<Uri>> GetServiceUris(string serviceName)
        {
            using (ConsulClient client = new ConsulClient(c =>
                   {
                       c.Address = _consulClientOptions.Address;
                       c.Datacenter = _consulClientOptions.Datacenter;
                   }))
            {
                var entrys = await client.Health.Service(serviceName);

                _logger.LogInformation(
                    $"{serviceName} form consul takes time：{entrys.RequestTime.TotalMilliseconds}ms");
                var uris = entrys.Response.Select(entry =>
                {
                    var service = entry.Service;
                    return new Uri($"http://{service.Address}:{service.Meta["GrpcPort"]}");
                });

                return uris.ToList();
            }
        }
        
    }
}