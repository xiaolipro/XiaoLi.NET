using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XiaoLi.NET.LoadBalancing
{
    public interface IResolver
    {
        /// <summary>
        /// 解析缓存刷新间隔
        /// </summary>
        TimeSpan RefreshInterval { get; }
        
        /// <summary>
        /// 解析服务（服务发现）
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<(List<Uri> serviceUris, dynamic metaData)> ResolutionService(string serviceName);
    }
}