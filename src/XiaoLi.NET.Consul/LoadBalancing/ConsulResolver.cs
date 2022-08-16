using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.LoadBalancing
{
    public class ConsulResolver : IResolver
    {
        private readonly ILogger<ConsulResolver> _logger;
        private readonly ConsulClientOptions _consulClientOptions;

        public ConsulResolver(ILogger<ConsulResolver> logger, IOptions<ConsulClientOptions> options)
        {
            _logger = logger;
            _consulClientOptions = options.Value?? throw new ArgumentNullException(nameof(ConsulClientOptions));
        }

        public string Name { get; } = nameof(ConsulResolver);
        public TimeSpan RefreshInterval { get; } = TimeSpan.FromSeconds(15);

        public async Task<(List<Uri> serviceUris, dynamic metaData)> ResolutionService(string serviceName)
        {
            using (ConsulClient client = new ConsulClient(c =>
                   {
                       c.Address = _consulClientOptions.Address;
                       c.Datacenter = _consulClientOptions.Datacenter;
                   }))
            {
                var entrys = await client.Health.Service(serviceName);

                var uris = entrys.Response.Select(x => new Uri($"http://{x.Service.Address}:{x.Service.Meta["GrpcPort"]}"));
                _logger.LogInformation(
                    "解析服务：{ServiceName} 成功：{Uris}，耗时：{RequestTimeTotalMilliseconds}ms", serviceName,
                    string.Join(",", uris) ,entrys.RequestTime.TotalMilliseconds);

                return (uris.ToList(), entrys.Response.Select(entry => entry.Service.Meta as dynamic).ToList());
            }
        }
    }
}