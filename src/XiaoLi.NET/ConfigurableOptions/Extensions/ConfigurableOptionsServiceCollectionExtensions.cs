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
using XiaoLi.NET.ConfigurableOptions.Attributes;

namespace XiaoLi.NET.ConfigurableOptions.Extensions
{
    /// <summary>
    /// 可配置项拓展类
    /// </summary>
    public static class ConfigurableOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// 添加配置项
        /// </summary>
        /// <typeparam name="TOptions">配置项类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services)
            where TOptions : class, IConfigurableOptions
        {
            var optionsType = typeof(TOptions);

            // 获取配置路径
            var path = GetConfigurationPath(optionsType);

            // 配置选项（含验证信息）
            var configurationRoot = App.Configuration;
            var optionsConfiguration = configurationRoot.GetSection(path);

            if (typeof(IMonitorConfigurableOptions<TOptions>).IsAssignableFrom(optionsType))
            {
                var onChangeMethod = optionsType.GetMethod(nameof(IMonitorConfigurableOptions<TOptions>.OnChange));
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
                .FirstOrDefault(x => x.IsGenericType && typeof(IConfigurableOptions).IsAssignableFrom(x.GetGenericTypeDefinition()));
            if (genericOptionsType != null)
            {
                // 泛型参数
                var genericArguments = genericOptionsType.GenericTypeArguments;

                // 验证
                if (genericArguments.Length > 1)
                {
                    // 注册验证器
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IValidateOptions<TOptions>), genericArguments.Last()));
                }

                // 配置后
                var postConfigureMethod = optionsType.GetMethod(nameof(IConfigurableOptions<TOptions>.PostConfigure));
                if (postConfigureMethod != null)
                {
                    services.PostConfigureAll<TOptions>(options => postConfigureMethod.Invoke(options, new object[] { options, optionsConfiguration }));
                }
            }

            return services;
        }

        /// <summary>
        /// 获取配置路径
        /// </summary>
        /// <param name="attr">配置特性</param>
        /// <param name="optionsType">配置类型</param>
        /// <returns></returns>
        private static string GetConfigurationPath(Type optionsType)
        {
            var attr = optionsType.GetCustomAttribute<ConfigurableOptionsAttribute>(false);

            if (attr != null)
            {
                if (!string.IsNullOrWhiteSpace(attr.Path))
                {
                    return attr.Path;
                }
            }
            
            // 默认后缀：Options
            string defaultStuffx = nameof(Options);

            // 切除后缀
            return optionsType.Name.Substring(0, optionsType.Name.Length - defaultStuffx.Length);//.AsSpan().Slice(0, optionsType.Name.Length - defaultStuffx.Length).ToString();
        }
    }
}
