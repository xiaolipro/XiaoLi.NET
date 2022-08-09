
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.ConfigurableOptions.Extensions;
using XiaoLi.NET.DependencyInjection.Extensions;

namespace XiaoLi.NET.Application.Extensions
{
    
    /// <summary>
    /// 框架服务拓展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加框架服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddApp(this IServiceCollection services)
        {
            // 添加App配置
            services.AddConfigurableOptions<AppSettingsOptions>();

            // 添加依赖注入
            services.AddDependencyInjection();

            // 添加Startup文件
            // services.AddStartups();

            return services;
        }
        //
        // private static IServiceCollection AddStartups(this IServiceCollection services)
        // {
        //     var startups = App.PublicTypes.Where(type =>
        //     {
        //         if (!type.IsClass) return false;
        //         if (type.IsAbstract || type.IsGenericType) return false;
        //
        //         return typeof(IStartup).IsAssignableFrom(type);
        //     });
        //
        //     foreach (var startupType in startups)
        //     {
        //         var startup = Activator.CreateInstance(startupType) as IStartup;
        //         InternalApp.Startups.Add(startup);
        //         
        //         // 公开的实例成员
        //         var configureServicesMethods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        //             // 返回void且第一个参数是IServiceCollection方法
        //             .Where(method =>
        //             {
        //                 if (method.ReturnType != typeof(void)) return false;
        //                 var parameters = method.GetParameters();
        //                 if (parameters.Length < 1) return false;
        //                 return parameters.First().ParameterType == typeof(IServiceCollection);
        //             });
        //
        //         // 调用ConfigureServices
        //         foreach (var configure in configureServicesMethods)
        //         {
        //             configure.Invoke(startup,new[] {services});
        //         }
        //     }
        //
        //     return services;
        // }
    }
}