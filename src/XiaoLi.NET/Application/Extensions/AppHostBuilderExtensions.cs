using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Internal;
using XiaoLi.NET.Configuration.Extensions;
using XiaoLi.NET.DependencyInjection.Extensions;
using XiaoLi.NET.Startup;
using XiaoLi.NET.Startup.Attributes;
using XiaoLi.NET.Startup.Extensions;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// App主机拓展，支持WebHost/Host
    /// </summary>
    public static class AppHostBuilderExtensions
    {
        /// <summary>
        /// 初始化Web主机 1.0+
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder InitWebApp(this IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    // 解析环境变量
#if NETCOREAPP3_0_OR_GREATER
                    InternalApp.WebHostEnvironment = InternalApp.ResolveWebEnvironmentVariables(context.HostingEnvironment);
#else
                    InternalApp.WebHostEnvironment =
                        InternalApp.ResolveWebEnvironmentVariables(context.HostingEnvironment);
#endif
                    
                    // 服务配置
                    InternalApp.Configuration = context.Configuration;
                    
                    // Startup
                    InternalApp.Startups = App.PublicTypes
                        .Where(x => typeof(IAutoStart).IsAssignableFrom(x) && !x.IsAbstract).OrderByDescending(x =>
                            x.IsDefined(typeof(StartOrderAttribute), false)
                                ? x.GetCustomAttribute<StartOrderAttribute>(false)!.Order
                                : 0)
                        .Select(x => Activator.CreateInstance(x) as IAutoStart);

                    // 添加json配置文件
                    InternalApp.AddJsonFiles(configurationBuilder);
                })
                .ConfigureServices(services =>
                {
                    // 管道模型
                    services.AddTransient<IStartupFilter, InternalStartupFilter>();

                    // 添加Options
                    services.AddAutoOptions();

                    // 添加依赖注入
                    services.AddDependencyInjection();

                    // 添加Startup
                    services.AddStartups();
                });

            return builder;
        }
        
        /// <summary>
        /// 初始化泛型主机 2.1+
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder InitApp(this IHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    // 解析环境变量
                    InternalApp.HostEnvironment = InternalApp.ResolveEnvironmentVariables(context.HostingEnvironment);
                    
                    // 服务配置
                    InternalApp.Configuration = context.Configuration;
                    
                    // Startup
                    InternalApp.Startups = App.PublicTypes
                        .Where(x => typeof(IAutoStart).IsAssignableFrom(x) && !x.IsAbstract).OrderByDescending(x =>
                            x.IsDefined(typeof(StartOrderAttribute), false)
                                ? x.GetCustomAttribute<StartOrderAttribute>(false)!.Order
                                : 0)
                        .Select(x => Activator.CreateInstance(x) as IAutoStart);

                    // 添加json配置文件
                    InternalApp.AddJsonFiles(configurationBuilder);
                })
                .ConfigureServices(services =>
                {
                    // 启动主机服务
                    services.AddHostedService<InternalHostedService>();

                    // 添加Options
                    services.AddAutoOptions();

                    // 添加依赖注入
                    services.AddDependencyInjection();

                    // 添加Startup
                    services.AddStartups();
                });

            return builder;
        }
    }
}