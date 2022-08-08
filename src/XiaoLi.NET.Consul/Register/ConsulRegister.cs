using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace XiaoLi.NET.Consul.Register
{
    public class ConsulRegister : IConsulRegister
    {
        private readonly ConsulClientOptions _consulClientOptions;
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        private readonly ILogger<ConsulRegister> _logger;

        public ConsulRegister(IOptions<ConsulClientOptions> consulClientOptions, IOptions<ConsulRegisterOptions> consulRegisterOptions, ILogger<ConsulRegister> logger)
        {
            _consulClientOptions = consulClientOptions.Value;
            _consulRegisterOptions = consulRegisterOptions.Value;
            _logger = logger;
        }

        public async Task Registry()
        {
            using (ConsulClient client = new ConsulClient(c =>
                   {
                       c.Address = _consulClientOptions.Address;  // Consul Uri
                       c.Datacenter = _consulClientOptions.Datacenter;
                   }))
            {
                await client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    ID = $"{_consulRegisterOptions.IP}:{_consulRegisterOptions.Port}（Start in {DateTime.Now}）",  // 服务唯一Id
                    Name = _consulRegisterOptions.ServiceName,  // 服务组名称
                    Address = _consulRegisterOptions.IP,  // 服务IP
                    Port = _consulRegisterOptions.Port,
                    Tags = _consulRegisterOptions.Tags,  // 一组标签
                    Check = new AgentServiceCheck()
                    {
                        Interval = TimeSpan.FromSeconds(_consulRegisterOptions.Interval),
                        HTTP = $"http://{_consulRegisterOptions.IP}:{_consulRegisterOptions.Port}/{_consulRegisterOptions.HealthCheckUrl.Trim('/')}",
                        Timeout = TimeSpan.FromSeconds(_consulRegisterOptions.Timeout),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(_consulRegisterOptions.DeregisterTime),  // 这个不配置挂掉的节点会一直在
                    },
                    Meta = new Dictionary<string, string>() { { "Weight", _consulRegisterOptions.Weight.ToString() } }  // 元数据
                });
                _logger.LogInformation($"{JsonConvert.SerializeObject(_consulRegisterOptions)} Consul注册已完成");
            }
        }
    }
}
