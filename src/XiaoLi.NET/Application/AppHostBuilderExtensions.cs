
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Internal;

namespace XiaoLi.NET.Application
{
    /// <summary>
    /// 框架主机构建拓展
    /// </summary>
    public static class AppHostBuilderExtensions
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
        
        /// <summary>
        /// 使用框架App
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder ConfigureApp(this IWebHostBuilder builder)
        {
            InternalApp.ConfigureApp(builder);
        
            return builder;
        }

    }
}