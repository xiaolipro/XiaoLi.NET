using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Application.Internal;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// WebHost.CreateDefaultBuilder()
    /// 配置 ASP.NET Core 应用程序的“原始”方法，截至 ASP.NET Core 2.x。
    /// </summary>
    public static class IWebHostBuilderExtensions
    {
        /// <summary>
        /// 使用框架App
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder ConfigureApp(this IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(((context, configurationBuilder) =>
                {
                    // 解析环境变量
                    InternalApp.HostingEnvironment = InternalApp.ResolveEnvironmentVariables(context.HostingEnvironment);
                    
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
                }));
        
            return builder;
        }
    }
}