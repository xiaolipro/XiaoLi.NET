using System;
using Grpc.Net.Client.Balancer;

namespace XiaoLi.NET.Grpc.LoadBalancingFactories;

/// <summary>
/// 为PollingResolver提供回退延迟
/// </summary>
public class CustomBackoffPolicyFactory:IBackoffPolicyFactory
{
    public IBackoffPolicy Create()
    {
        return new CustomBackoffPolicy();
    }
    
    
    private class CustomBackoffPolicy:IBackoffPolicy
    {
        public TimeSpan NextBackoff()
        {
            return TimeSpan.FromSeconds(3);
        }
    }
}