using System;
using System.Collections.Generic;
using System.Linq;
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

        public ConsulRegister(IOptions<ConsulClientOptions> consulClientOptions,
            IOptions<ConsulRegisterOptions> consulRegisterOptions, ILogger<ConsulRegister> logger)
        {
            _consulClientOptions = consulClientOptions.Value;
            _consulRegisterOptions = consulRegisterOptions.Value;
            _logger = logger;
        }

        public async Task Registry()
        {
            using (var client = new ConsulClient(c =>
                   {
                       c.Address = _consulClientOptions.Address; // Consul Uri
                       c.Datacenter = _consulClientOptions.Datacenter;
                   }))
            {
                bool isDuplicate = await DuplicateRegistration(client);
                if (isDuplicate)
                {
                    _logger.LogInformation($"{JsonConvert.SerializeObject(_consulRegisterOptions)} 已被注册过");
                    return;
                }

                await client.Agent.ServiceRegister(BuiltRegistration());
                _logger.LogInformation($"{JsonConvert.SerializeObject(_consulRegisterOptions)} Consul注册已完成");
            }
        }

        private AgentServiceRegistration BuiltRegistration()
        {
            string healthCheckUrl =
                $"http://{_consulRegisterOptions.IP}:{_consulRegisterOptions.Port}/{_consulRegisterOptions.HealthCheckRoute.Trim('/')}";
            var httpHealthCheck = new AgentServiceCheck()
            {
                Interval = TimeSpan.FromSeconds(_consulRegisterOptions.Interval), // 健康检查时间间隔
                HTTP = healthCheckUrl,
                Timeout = TimeSpan.FromSeconds(_consulRegisterOptions.Timeout), // 心跳超时时间
                DeregisterCriticalServiceAfter =
                    TimeSpan.FromSeconds(_consulRegisterOptions.DeregisterTime), // 服务挂掉多久后注销，这个不配置挂掉的节点会一直在
            };
            
            var registration= new AgentServiceRegistration()
            {
                ID = $"{_consulRegisterOptions.IP}:{_consulRegisterOptions.Port}（Start in {DateTime.Now}）", // 服务唯一Id
                Name = _consulRegisterOptions.ServiceName, // 服务组名称
                Address = _consulRegisterOptions.IP, // 服务IP
                Port = _consulRegisterOptions.Port,
                Tags = _consulRegisterOptions.Tags, // 一组标签
                Check = httpHealthCheck,
                Meta = new Dictionary<string, string>()
                    { { "Weight", _consulRegisterOptions.Weight.ToString() } } // 元数据
            };

            // grpc心跳
            if (_consulRegisterOptions.GrpcHelthCheck)
            {
                registration.Check.GRPC = healthCheckUrl;
                registration.Check.GRPCUseTLS = false; // 是否同时支持http
            }

            return registration;
        }

        private async Task<bool> DuplicateRegistration(ConsulClient client)
        {
            var services = await client.Catalog.Service(_consulRegisterOptions.ServiceName);

            return services.Response.Any(x =>
                x.ServiceAddress == _consulRegisterOptions.IP && x.ServicePort == _consulRegisterOptions.Port);
        }
    }
}