using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XiaoLi.NET.Consul.Dispatcher;
using XiaoLi.NET.Consul.Register;

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
        /// 添加Consul调度器，提供服务发现
        /// </summary>
        public static void AddConsulDispatcher<TDispatcher>(this IServiceCollection services) where TDispatcher : AbstractConsulDispatcher
        {
            services.TryAddTransient(typeof(AbstractConsulDispatcher),typeof(TDispatcher));
        }

    }
}
