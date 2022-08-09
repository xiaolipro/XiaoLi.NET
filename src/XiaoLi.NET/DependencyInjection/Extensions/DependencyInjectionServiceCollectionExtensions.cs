using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XiaoLi.NET.Application;
using XiaoLi.NET.ConfigurableOptions.Extensions;
using XiaoLi.NET.DependencyInjection.Attributes;
using XiaoLi.NET.DependencyInjection.Enums;
using XiaoLi.NET.DependencyInjection.LifecycleInterfaces;

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
            var lifecycleInterfaces = new[] { typeof(ISingleton), typeof(IScoped), typeof(ITransient) };
            
            // 获取所有需要注入的实现类
            var implementationTypes = App.PublicTypes
                .Where(type =>
                {
                    if (!type.IsClass) return false;
                    if (type.IsAbstract) return false;

                    return lifecycleInterfaces.Any(lifecycleInterface => lifecycleInterface.IsAssignableFrom(type));
                });



            foreach (var implementationType in implementationTypes)
            {
                var interfaces = implementationType.GetInterfaces();

                var attr = new DependencyInjectionAttribute();
                if (implementationType.IsDefined(typeof(DependencyInjectionAttribute), false))
                {
                    attr = implementationType.GetCustomAttribute<DependencyInjectionAttribute>();
                }
                else
                {
                    var lifecycleType = interfaces.Last(x => lifecycleInterfaces.Contains(x));
                    attr.Lifecycle = (ServiceLifecycle)Enum.Parse(typeof(ServiceLifecycle), lifecycleType.Name.TrimStart('I'));
                }

                IEnumerable<Type> injectableInterfaces;
                if (implementationType.IsDefined(typeof(ExposeServicesAttribute), false))
                {
                    injectableInterfaces = implementationType.GetCustomAttribute<ExposeServicesAttribute>().Interfaces;
                }
                else
                {
                    injectableInterfaces = interfaces.Where(x =>
                    {
                        if (lifecycleInterfaces.Contains(x)) return false;
                        if (x.IsGenericType != implementationType.IsGenericType) return false;
                        return x.GetGenericArguments().Length == implementationType.GetGenericArguments().Length;
                    });
                }

                services.RegisterService(implementationType, injectableInterfaces, attr);
            }


            return services;
        }

        private static IServiceCollection RegisterService(this IServiceCollection services,
            Type implementationType, IEnumerable<Type> serviceTypes,
            DependencyInjectionAttribute attr)
        {
            var registerPolicy = attr.RegisterPolicy;
            switch (registerPolicy)
            {
                case RegisterPolicy.OnlyImplement:
                    services.RegisterService(implementationType, attr);
                    break;
                case RegisterPolicy.FirstInterface:
                {
                    var serviceType = serviceTypes.FirstOrDefault();
                    services.RegisterService(implementationType, attr, serviceType);
                    break;
                }
                case RegisterPolicy.NamingConventionsInterface:
                {
                    var serviceType = serviceTypes.FirstOrDefault(x => x.Name.Equals("I" + implementationType.Name));
                    services.RegisterService(implementationType, attr, serviceType);
                    break;
                }
                case RegisterPolicy.AllInterfaces:
                {
                    foreach (var serviceType in serviceTypes)
                    {
                        services.RegisterService(implementationType, attr, serviceType);
                    }
                    break;
                }
                default:
                    goto case RegisterPolicy.NamingConventionsInterface;
            }

            return services;
        }

        private static IServiceCollection RegisterService(this IServiceCollection services,
            Type implementationType,
            DependencyInjectionAttribute attr, Type serviceType = null)
        {
            
            if (serviceType != null)
            {
                if (attr.ReplaceService)
                {
                    services.Replace(new ServiceDescriptor(serviceType, implementationType,(ServiceLifetime)attr.Lifecycle));
                }
                else
                {
                    switch (attr.Lifecycle)
                    {
                        case ServiceLifecycle.Transient:
                            services.TryAddTransient(serviceType, implementationType);
                            break;
                        case ServiceLifecycle.Scoped:
                            services.TryAddScoped(serviceType, implementationType);
                            break;
                        case ServiceLifecycle.Singleton:
                            services.TryAddSingleton(serviceType, implementationType);
                            break;
                        default:
                            goto case ServiceLifecycle.Transient;
                    }
                }
            }
            else
            {
                if (attr.ReplaceService)
                {
                    services.Replace(new ServiceDescriptor(implementationType, implementationType,(ServiceLifetime)attr.Lifecycle));
                }
                switch (attr.Lifecycle)
                {
                  
                    case ServiceLifecycle.Transient:
                        services.TryAddTransient(implementationType);
                        break;
                    case ServiceLifecycle.Scoped:
                        services.TryAddScoped(implementationType);
                        break;
                    case ServiceLifecycle.Singleton:
                        services.TryAddSingleton(implementationType);
                        break;
                    default:
                        goto case ServiceLifecycle.Transient;
                }
            }

            return services;
        }
    }
}