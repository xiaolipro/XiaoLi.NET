using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace XiaoLi.NET.App.DependencyInjection.Extensions
{
    /// <summary>
    /// 依赖注入拓展类
    /// </summary>
    public static class DependencyInjectionServiceCollectionExtensions
    {
        /// <summary>
        /// 添加依赖注入
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            // 添加外部程序集配置
            services.AddConfigurableOptions<DependencyInjectionSettingsOptions>();

            services.AddInnerDependencyInjection();
            return services;
        }
    }
}
