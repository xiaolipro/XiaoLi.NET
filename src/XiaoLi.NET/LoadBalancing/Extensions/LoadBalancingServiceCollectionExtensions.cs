using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace XiaoLi.NET.LoadBalancing.Extensions
{
    public static class LoadBalancingServiceCollectionExtensions
    {
        /// <summary>
        /// 添加客户端负载均衡器
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TResolver"></typeparam>
        /// <typeparam name="TBalancer"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddClientLoadBalancer<TResolver, TBalancer>(
            this IServiceCollection services)
            where TResolver : class, IResolver
            where TBalancer : class, IBalancer
        {
            services.TryAddSingleton<IResolver,TResolver>();
            services.TryAddSingleton<IBalancer,TBalancer>();
            // services.Replace(new ServiceDescriptor(typeof(IBalancer), typeof(TBalancer),
            //     ServiceLifetime.Singleton));
            // services.Replace(new ServiceDescriptor(typeof(IResolver), typeof(TResolver),
            //     ServiceLifetime.Singleton));

            return services;
        }
    }
}