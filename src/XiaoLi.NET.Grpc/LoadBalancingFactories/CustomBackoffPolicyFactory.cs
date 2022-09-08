using System;
using Grpc.Net.Client.Balancer;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories
{
    /// <summary>
    /// 为PollingResolver提供回退延迟
    /// 解决后台任务（客户端）先起，服务端未起的问题，客户端解析不到服务，会根据回退策略提供的时间，不断重试。
    /// </summary>
    public class CustomBackoffPolicyFactory : IBackoffPolicyFactory
    {
        public IBackoffPolicy Create()
        {
            return new CustomBackoffPolicy();
        }


        private class CustomBackoffPolicy : IBackoffPolicy
        {
            public TimeSpan NextBackoff()
            {
                return TimeSpan.FromSeconds(5);
            }
        }
    }
}