using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XiaoLi.NET.Consul.Dispatcher;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.Consul.Register;
using XiaoLi.NET.LoadBalancing;

namespace XiaoLi.NET.Consul.Extensions
{
    public static class ConsulServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Consul注册机加入到IOC容器
        /// </summary>
        /// <param name="configuration"></param>
        public static void AddConsulRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulClientOptions>(configuration.GetSection("ConsulClient"));
            services.Configure<ConsulRegisterOptions>(configuration.GetSection("ConsulRegister"));
            services.AddTransient<IConsulRegister, ConsulRegister>();
        }


        /// <summary>
        /// 添加Consul负载均衡调度器
        /// </summary>
        public static void AddConsulDispatcher<TBalancer>(this IServiceCollection services) where TBalancer : class, IBalancer
        {
            services.TryAddSingleton<IBalancer,TBalancer>();
            services.TryAddSingleton<IResolver, ConsulResolver>();
            services.TryAddSingleton<IDispatcher,ConsulDispatcher>();
        }

    }
}
