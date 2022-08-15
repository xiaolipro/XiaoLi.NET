
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.Exceptions;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// ConsulDispatcher基类
    /// </summary>
    public abstract class ConsulDispatcher:IDispatcher
    {
        private readonly ILogger<ConsulDispatcher> _logger;
        private readonly IResolver _resolver;
        private readonly IBalancer _balancer;

        public ConsulDispatcher(ILogger<ConsulDispatcher> logger, IResolver resolver, IBalancer balancer)
        {
            _logger = logger;
            _resolver = resolver;
            _balancer = balancer;
        }

        /// <summary>
        /// 根据服务名称获取调度后的真实地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> GetRealHostAsync(string serviceName)
        {
            return await ChooseHostAsync(serviceName);
        }

        /// <summary>
        /// 根据服务选择一台主机
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>主机：IP+Port</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private async Task<string> ChooseHostAsync(string serviceName)
        {
            var agentServices = await _resolver.ResolutionService(serviceName);

            int index = _balancer.Pick(agentServices);
            if (index >= agentServices.Count) throw new IndexOutOfRangeException();

            var service = agentServices[index];
            return $"{service.Address}:{service.Port}";
        }
    }

}