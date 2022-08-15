
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Consul.Exceptions;
using XiaoLi.NET.Consul.LoadBalancing;

namespace XiaoLi.NET.Consul.Dispatcher
{
    /// <summary>
    /// ConsulDispatcher基类
    /// </summary>
    public abstract class AbstractConsulDispatcher
    {
        private readonly ILogger<AbstractConsulDispatcher> _logger;
        private readonly ConsulResolver _consulResolver;

        public AbstractConsulDispatcher(ILogger<AbstractConsulDispatcher> logger, ConsulResolver consulResolver)
        {
            _logger = logger;
            _consulResolver = consulResolver;
        }

        protected List<AgentService> AgentServices;

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
        protected virtual async Task<string> ChooseHostAsync(string serviceName)
        {
            AgentServices = await _consulResolver.InternalResolutionService(serviceName);
            int count = AgentServices.Count;
            if (count == 0) throw new NotFindServiceException();

            int index = GetBalancedIndex(count);
            if (index >= count) throw new IndexOutOfRangeException();

            var service = AgentServices[index];
            return $"{service.Address}:{service.Port}";
        }

        internal abstract int GetBalancedIndex(int serviceCount);
    }

}