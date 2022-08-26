using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using XiaoLi.NET.Application;
using XiaoLi.NET.Configuration.Attributes;

namespace XiaoLi.NET.Configuration.Extensions
{
    /// <summary>
    /// 可配置项拓展类
    /// </summary>
    public static class ConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoOptions(this IServiceCollection services)
        {
            var configuration = App.Configuration;

            var types = App.PublicTypes
                .Where(type => typeof(IAutoOptions).IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract);
            foreach (var optionsType in types)
            {
                #region IConfigureOptions

                // 获取配置路径
                string path = App.GetConfigurationPath(optionsType);

                var section = configuration.GetSection(path);

                typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                        new[] { typeof(IServiceCollection), typeof(IConfiguration) })
                    ?.MakeGenericMethod(optionsType)
                    // 静态方法忽略obj参数
                    .Invoke(null, new object[] { services, section });


                var genericOptionsType = optionsType.GetInterfaces()
                    .FirstOrDefault(x =>
                        x.IsGenericType && typeof(IAutoOptions).IsAssignableFrom(x.GetGenericTypeDefinition()));

                if (genericOptionsType == null) continue;

                #endregion

                #region IPostConfigureOptions 配置后的

                var actionType = typeof(Action<>).MakeGenericType(optionsType);
                var postConfigureMethod =
                    typeof(IAutoOptions<>).MakeGenericType(optionsType)
                        .GetMethod(nameof(IAutoOptions<AppOptions>.PostConfigure));

                var instance = App.Configuration.GetSection(path).Get(optionsType);
                var actionExpress = Delegate.CreateDelegate(actionType, instance, postConfigureMethod);

                var postConfigureHandler = typeof(OptionsServiceCollectionExtensions)
                    .GetMethod(nameof(OptionsServiceCollectionExtensions.PostConfigure),
                        new[] { typeof(IServiceCollection), typeof(Action<>) })
                    ?.MakeGenericMethod(optionsType);
                postConfigureHandler?.Invoke(null, new object[] { services, actionExpress });

                #endregion

                #region IValidateOptions 带验证的

                var genericTypeArguments = optionsType.GenericTypeArguments;
                if (genericTypeArguments.Length > 1)
                {
                    // 注册验证器
                    services.TryAddSingleton(typeof(IValidateOptions<>).MakeGenericType(optionsType),
                        genericTypeArguments.Last());
                }

                #endregion
            }

            return services;
        }


        /// <summary>
        /// 添加配置项
        /// TODO: 自定义版本的options第一个版本先不做了
        /// </summary>
        /// <typeparam name="TOptions">配置项类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        internal static IServiceCollection AddConfiguration<TOptions>(this IServiceCollection services)
            where TOptions : class, IAutoOptions
        {
            var optionsType = typeof(TOptions);

            // 获取配置路径
            var path = App.GetConfigurationPath<TOptions>();

            // 配置选项（含验证信息）
            var configurationRoot = App.Configuration;
            var optionsConfiguration = configurationRoot.GetSection(path);

            if (typeof(IAutoMonitorOptions<TOptions>).IsAssignableFrom(optionsType))
            {
                var onChangeMethod = optionsType.GetMethod(nameof(IAutoMonitorOptions<TOptions>.OnChange));
                if (onChangeMethod != null)
                {
                    // 监听配置文件变化
                    ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
                    {
                        var options = optionsConfiguration.Get<TOptions>();
                        if (options != null) onChangeMethod.Invoke(options, new object[] { options });
                    });
                }
            }

            var genericOptionsType = optionsType.GetInterfaces()
                .FirstOrDefault(x =>
                    x.IsGenericType && typeof(IAutoOptions).IsAssignableFrom(x.GetGenericTypeDefinition()));
            if (genericOptionsType != null)
            {
                // 泛型参数
                var genericArguments = genericOptionsType.GenericTypeArguments;

                // 带验证的
                if (genericArguments.Length > 1)
                {
                    // 注册验证器
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IValidateOptions<TOptions>),
                        genericArguments.Last()));
                }

                // 配置后
                var postConfigureMethod = optionsType.GetMethod(nameof(IAutoOptions<TOptions>.PostConfigure));
                if (postConfigureMethod != null)
                {
                    services.PostConfigureAll<TOptions>(options =>
                        postConfigureMethod.Invoke(options, new object[] { options, optionsConfiguration }));
                }
            }

            return services;
        }
    }
}