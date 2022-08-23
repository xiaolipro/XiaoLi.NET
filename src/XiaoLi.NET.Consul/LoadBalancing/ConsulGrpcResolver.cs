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
    public class ConsulGrpcResolver : IResolver
    {
        private readonly ILogger<ConsulGrpcResolver> _logger;
        private readonly ConsulClientOptions _consulClientOptions;

        public ConsulGrpcResolver(ILogger<ConsulGrpcResolver> logger, IOptions<ConsulClientOptions> options)
        {
            _logger = logger;
            _consulClientOptions = options.Value ?? throw new ArgumentNullException(nameof(ConsulClientOptions));
        }

        public string Name { get; } = nameof(ConsulGrpcResolver);
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

                var uris = entrys.Response
                    .Select(x => new Uri($"http://{x.Service.Address}:{x.Service.Meta["GrpcPort"]}")).ToList();

                if (uris.Count < 1)
                {
                    _logger.LogWarning("未能从服务：{ServiceName} 中解析到任何实例，耗时：{RequestTimeTotalMilliseconds}ms", serviceName,
                        entrys.RequestTime.TotalMilliseconds);
                }
                else
                {
                    _logger.LogInformation(
                        "解析服务：{ServiceName} 成功：{Uris}，耗时：{RequestTimeTotalMilliseconds}ms", serviceName,
                        string.Join(",", uris), entrys.RequestTime.TotalMilliseconds);
                }

                return (uris, entrys.Response.Select(entry => entry.Service.Meta as dynamic).ToList());
            }
        }
    }
}