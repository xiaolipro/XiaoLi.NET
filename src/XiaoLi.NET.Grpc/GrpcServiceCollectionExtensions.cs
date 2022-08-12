using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Grpc.Interceptors;
using XiaoLi.NET.Grpc.LoadBalancers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XiaoLi.NET.Grpc
{
    public static class GrpcServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Grpc客户端
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="services"></param>
        /// <param name="address"></param>
        public static void AddGrpcClientDependsOnConsul<TClient>(this IServiceCollection services, string address) where TClient : class
        {
            services.TryAddSingleton<ResolverFactory, GrpcResolverFactory>();
            services.TryAddSingleton<LoadBalancerFactory, GrpcBalancerFactory>();

            services
                .AddGrpcClient<TClient>(options =>
                {
                    options.Address = new Uri("consul:///" + address);
                })
                .ConfigureChannel(options =>
                {
                    options.Credentials = ChannelCredentials.Insecure;
                    options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("consul") } };
                    options.ServiceProvider = services.BuildServiceProvider();
                })
                .AddInterceptor<ClientLogInterceptor>();
        }
    }
}
