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
        /// 添加Grpc负载均衡客户端
        /// </summary>
        /// <typeparam name="TClient"></typeparam>
        /// <param name="services"></param>
        /// <param name="address"></param>
        public static IHttpClientBuilder AddGrpcLoadBalancingClient<TClient,TLoadBalancer>(this IServiceCollection services, string address) 
            where TClient : class
            where TLoadBalancer : ILoadBalancer
        {
            services.AddGrpcClientLoadBalancer<TLoadBalancer>();
            
            services.TryAddSingleton(typeof(ClientLogInterceptor));
            services.TryAddSingleton(typeof(ClientExceptionInterceptor));
            return services
                .AddGrpcClient<TClient>(options =>
                {
                    options.Address = new Uri($"{typeof(TLoadBalancer).Name}://" + address);
                })
                .ConfigureChannel(options =>
                {
                    options.Credentials = ChannelCredentials.Insecure;
                    options.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new LoadBalancingConfig(typeof(TLoadBalancer).Name) } };
                    options.ServiceProvider = services.BuildServiceProvider();
                })
                .AddInterceptor<ClientExceptionInterceptor>()
                .AddInterceptor<ClientLogInterceptor>();
        }

        /// <summary>
        /// 添加Grpc客户端负载均衡器
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TLoadBalancer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddGrpcClientLoadBalancer<TLoadBalancer>(this IServiceCollection services)
            where TLoadBalancer : ILoadBalancer
        {
            services.Replace(new ServiceDescriptor(typeof(ILoadBalancer), typeof(TLoadBalancer),
                ServiceLifetime.Singleton));
            
            services.TryAddSingleton<ResolverFactory, CustomResolverFactory>();
            services.TryAddSingleton<LoadBalancerFactory, CustomBalancerFactory>();

            return services;
        }
    }
}
