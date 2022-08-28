using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Internal;
using XiaoLi.NET.Configuration.Extensions;
using XiaoLi.NET.DependencyInjection.Extensions;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// 2.1, 2.2, 3.0, 3.1, 5, 6, 7 Preview 7
    /// </summary>
    public static class IHostBuilderExtensions
    {
        /// <summary>
        /// 使用框架App
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder InitApp(this IHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(((context, configurationBuilder) =>
                {
                    // 解析环境变量
                    InternalApp.HostEnvironment = InternalApp.ResolveEnvironmentVariables(context.HostingEnvironment);
                    
                    // 添加json配置文件
                    InternalApp.AddJsonFiles(configurationBuilder);
                }))
                .ConfigureServices(((context, services) =>
                {
                    // 服务配置
                    InternalApp.Configuration = context.Configuration;
                    
                    // 服务存储容器
                    InternalApp.Services = services;
                    
                    // 初始化根服务
                    services.AddHostedService<InternalHostedService>();

                    // 添加Options
                    services.AddAutoOptions();
                    
                    // 添加依赖注入
                    services.AddDependencyInjection();
                }));

            return builder;
        }
    }
}