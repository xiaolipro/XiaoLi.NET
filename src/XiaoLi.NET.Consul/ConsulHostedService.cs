using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using XiaoLi.NET.Application;

namespace XiaoLi.NET.Consul
{
    public class ConsulHostedService: IHostedService
    {
        private readonly ILogger<ConsulHostedService> _logger;
        private readonly IConsulClient _consulClient;
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        private CancellationTokenSource _consulCancellationToken;
        private string _serviceId;

        public ConsulHostedService(ILogger<ConsulHostedService> logger,IConsulClient consulClient,IOptions<ConsulRegisterOptions> options)
        {
            _logger = logger;
            _consulClient = consulClient;
            _consulRegisterOptions =  options.Value;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _consulCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            #region 服务注销，防止重复注册

            var services = await _consulClient.Catalog.Service(_consulRegisterOptions.ServiceName, cancellationToken);

            var targets = services.Response.Where(x =>
                x.ServiceAddress == _consulRegisterOptions.IP && x.ServicePort == _consulRegisterOptions.Port);

            foreach(var service in targets)
            {
                await _consulClient.Agent.ServiceDeregister(service.ServiceID, cancellationToken);
            }

            #endregion
                
            
            var registration = BuildRegistration();
            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
            _logger.LogInformation("{SerializeObject} Consul注册已完成", JsonConvert.SerializeObject(_consulRegisterOptions));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _consulCancellationToken.Cancel();
            await _consulClient.Agent.ServiceDeregister(_serviceId, cancellationToken);
            _logger.LogInformation("Consul已注销");
        }
        
        
        
        private AgentServiceRegistration BuildRegistration()
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

            _serviceId = $"{_consulRegisterOptions.IP}:{_consulRegisterOptions.Port}（Start in {DateTime.Now}）";
            var registration = new AgentServiceRegistration()
            {
                ID = _serviceId, // 服务唯一Id
                Name = _consulRegisterOptions.ServiceName, // 服务组名称
                Address = _consulRegisterOptions.IP, // 服务IP
                Port = _consulRegisterOptions.Port,
                Tags = _consulRegisterOptions.Tags, // 一组标签
                Check = httpHealthCheck,
                Meta = new Dictionary<string, string>()
                {
                    { nameof(_consulRegisterOptions.Weight), _consulRegisterOptions.Weight.ToString() },
                    { nameof(_consulRegisterOptions.GrpcPort), _consulRegisterOptions.GrpcPort.ToString() }
                } // 元数据
            };

            // grpc心跳
            if (_consulRegisterOptions.GrpcHelthCheck)
            {
                registration.Check.GRPC = healthCheckUrl;
                registration.Check.GRPCUseTLS = false; // 支持http
            }

            return registration;
        }
    }
}