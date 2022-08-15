using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancing
{
    /// <summary>
    /// 均衡器
    /// </summary>
    public interface IBalancer
    {
        /// <summary>
        /// 均衡器名称
        /// </summary>
        string Name { get; }

        
        /// <summary>
        /// 均衡算法
        /// </summary>
        /// <param name="services">服务数量</param>
        /// <returns></returns>
        int Pick(List<dynamic> services);
    }
}