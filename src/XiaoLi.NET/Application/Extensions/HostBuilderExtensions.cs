
using Microsoft.Extensions.Hosting;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// 框架主机构建拓展
    /// </summary>
    public static class HostBuilderExtensions
    {
        
        /// <summary>
        /// 使用框架App
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            InternalApp.ConfigureApp(builder);

            return builder;
        }
        // public static IHostBuilder CreateAppHostBuilder() => CreateAppHostBuilder(null);
        // public static IHostBuilder CreateAppHostBuilder(string[] args)
        // {
        //     return Host.CreateDefaultBuilder().ConfigureApp();
        // }
        

        
    }
}