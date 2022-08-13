using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancers
{
    public interface IGrpcLoadBalancer
    {
        /// <summary>
        /// 均衡器名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 均衡结果
        /// </summary>
        /// <returns></returns>
        int GetBalancedIndex(int serviceCount);

        /// <summary>
        /// 均衡目标
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<Uri>> GetServiceUris(string serviceName);
    }
}