using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Hosting;
using XiaoLi.NET.Application.Internal;
using XiaoLi.NET.Configuration.Extensions;
using XiaoLi.NET.DependencyInjection.Extensions;
using XiaoLi.NET.Startup;
using XiaoLi.NET.Startup.Attributes;
using XiaoLi.NET.Startup.Extensions;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// 2.1, 2.2, 3.0, 3.1, 5, 6, 7 Preview 7
    /// </summary>
    public static class AppHostBuilderExtensions
    {
        /// <summary>
        /// 使用框架App
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder InitWebApp(this IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    // 解析环境变量
                    InternalApp.HostEnvironment = default;//InternalApp.ResolveEnvironmentVariables(context.HostingEnvironment);
                    
                    // 服务配置
                    InternalApp.Configuration = context.Configuration;
                    
                    // Startup
                    InternalApp.Startups = App.PublicTypes
                        .Where(x => typeof(IAutoStart).IsAssignableFrom(x) && !x.IsAbstract).OrderByDescending(x =>
                            x.IsDefined(typeof(StartOrderAttribute), false)
                                ? x.GetCustomAttribute<StartOrderAttribute>(false).Order
                                : 0)
                        .Select(x => Activator.CreateInstance(x) as IAutoStart);

                    // 添加json配置文件
                    InternalApp.AddJsonFiles(configurationBuilder);
                })
                .ConfigureServices(services =>
                {
                    // 管道模型
                    services.AddTransient<IStartupFilter, InternalStartupFilter>();
                    // // 初始化根服务
                    // services.AddHostedService<InternalHostedService>();

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