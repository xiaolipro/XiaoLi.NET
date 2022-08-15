using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancing
{
    public interface IBalancer
    {
        /// <summary>
        /// 均衡器名称
        /// </summary>
        string Name { get; }

        
        /// <summary>
        /// 均衡结果
        /// </summary>
        /// <param name="serviceCount">服务数量</param>
        /// <returns></returns>
        int Pick(int serviceCount);
    }
}