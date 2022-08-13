using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Grpc.Interceptors;
using XiaoLi.NET.Grpc.LoadBalancers;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XiaoLi.NET.LoadBalancers;
using Microsoft.Extensions.Configuration;

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
        public static IHttpClientBuilder AddCustomeGrpcClient<TClient>(this IServiceCollection services, string address) where TClient : class
        {
            services.TryAddSingleton(typeof(ClientLogInterceptor));
            return services
                .AddGrpcClient<TClient>(options =>
                {
                    options.Address = new Uri("consul://" + address);
                })
                .ConfigureChannel(options =>
                {
                    options.Credentials = ChannelCredentials.Insecure;
                    options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig("consul") } };
                    options.ServiceProvider = services.BuildServiceProvider();
                })
                .AddInterceptor<ClientLogInterceptor>();
        }

        public static IServiceCollection AddGrpcClientLoadBalancer<TLoadBalancer>(this IServiceCollection services)
            where TLoadBalancer : IGrpcLoadBalancer
        {
            services.Replace(new ServiceDescriptor(typeof(IGrpcLoadBalancer), typeof(TLoadBalancer),
                ServiceLifetime.Singleton));
            
            services.TryAddSingleton<ResolverFactory, CustomResolverFactory>();
            services.TryAddSingleton<LoadBalancerFactory, CustomBalancerFactory>();

            return services;
        }
    }
}
