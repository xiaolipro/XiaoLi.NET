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
        /// <param name="serviceCount">服务数量</param>
        /// <param name="metaData">元数据</param>
        /// <returns></returns>
        int Pick(int serviceCount,dynamic metaData = default);
    }
}