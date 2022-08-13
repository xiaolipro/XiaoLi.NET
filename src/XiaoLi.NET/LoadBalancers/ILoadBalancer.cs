using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancers
{
    public interface ILoadBalancer
    {
        /// <summary>
        /// 均衡器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 刷新间隔
        /// </summary>
        TimeSpan RefreshInterval { get; }
        
        /// <summary>
        /// 均衡结果
        /// </summary>
        /// <returns></returns>
        int GetBalancedIndex(int serviceCount);

        /// <summary>
        /// 解析服务（服务发现）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<Uri>> ResolutionService(string serviceName);
    }
}