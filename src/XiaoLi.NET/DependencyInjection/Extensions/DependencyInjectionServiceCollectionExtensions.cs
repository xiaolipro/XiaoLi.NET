using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Application;
using XiaoLi.NET.ConfigurableOptions.Extensions;
using XiaoLi.NET.DependencyInjection.Attributes;
using XiaoLi.NET.DependencyInjection.Enums;

namespace XiaoLi.NET.DependencyInjection.Extensions
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
            services.AddConfigurableOptions<DependencyInjectionOptions>();

            services.AddInnerDependencyInjection();
            return services;
        }

        private static IServiceCollection AddInnerDependencyInjection(this IServiceCollection services)
        {
            // 获取所有需要注入的类型
            var injectionTypes = App.PublicTypes
                .Where(type => typeof(ISingletonDependency).IsAssignableFrom(type) ||
                               typeof(IScopedDependency).IsAssignableFrom(type) ||
                               typeof(ITransientDependency).IsAssignableFrom(type));

            
            foreach (var type in injectionTypes)
            {
                var interfaces = type.GetInterfaces();

                var attr = new DependencyInjectionAttribute();
                if (type.IsDefined(typeof(DependencyInjectionAttribute), false))
                {
                    attr = type.GetCustomAttribute<DependencyInjectionAttribute>();
                }
                else
                {
                    var lifecycleType = interfaces.Last(x => Enum.IsDefined(typeof(ServiceLifecycle), x.Name));
                    attr.Lifecycle =(ServiceLifecycle)Enum.Parse(typeof(ServiceLifecycle),lifecycleType.Name);
                }

                var injectableInterfaces = Enumerable.Empty<Type>();
                if  (type.IsDefined(typeof(ExposeServicesAttribute), false))
                {
                    injectableInterfaces = type.GetCustomAttribute<ExposeServicesAttribute>().Interfaces;
                }
                else
                {
                    injectableInterfaces = interfaces.Where(x =>
                    {
                        if (Enum.IsDefined(typeof(ServiceLifecycle), x.Name)) return false;
                        if (x.IsGenericType != type.IsGenericType) return false;
                        return x.GetGenericArguments().Length == type.GetGenericArguments().Length;
                    });
                }
            }
            
            services.RegisterService(type, injec)

            return services;
        }
    }
}