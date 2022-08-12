using System;
using System.Collections;
using System.Collections.Generic;

namespace XiaoLi.NET.Grpc.LoadBalancers
{
    public interface IGrpcLoadBalancer
    {
        string Name { get; }

        int GetIndex();

        IEnumerable<Uri> GetServiceUris(string serviceName);
    }
}