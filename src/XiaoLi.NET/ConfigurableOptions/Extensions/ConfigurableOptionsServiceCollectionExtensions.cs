using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using XiaoLi.NET.App.ConfigurableOptions.Attributes;

namespace XiaoLi.NET.App.ConfigurableOptions.Extensions
{
    /// <summary>
    /// 可配置项拓展类
    /// </summary>
    public static class ConfigurableOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// 添加选项配置
        /// </summary>
        /// <typeparam name="TOptions">选项类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services)
            where TOptions : class, IConfigurableOptions
        {
            var optionsType = typeof(TOptions);
            var optionsSettings = optionsType.GetCustomAttribute<ConfigurableOptionsAttribute>(false);

            // 获取配置路径
            var path = GetConfigurationPath(optionsSettings, optionsType);

            // 配置选项（含验证信息）
            var configurationRoot = App.Configuration;
            var optionsConfiguration = configurationRoot.GetSection(path);

            // 配置选项监听
            if (typeof(IConfigurableOptionsListener<TOptions>).IsAssignableFrom(optionsType))
            {
                var onListenerMethod = optionsType.GetMethod(nameof(IConfigurableOptionsListener<TOptions>.OnListener));
                if (onListenerMethod != null)
                {
                    // 这里监听的是全局配置（总感觉不对头）
                    ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
                    {
                        var options = optionsConfiguration.Get<TOptions>();
                        if (options != null) onListenerMethod.Invoke(options, new object[] { options, optionsConfiguration });
                    });
                }
            }

            var optionsConfigure = services.AddOptions<TOptions>()
                  .Bind(optionsConfiguration, options =>
                  {
                      options.BindNonPublicProperties = true; // 绑定私有变量
                  })
                  .ValidateDataAnnotations();

            // 配置复杂验证后后期配置
            var validateInterface = optionsType.GetInterfaces()
                .FirstOrDefault(u => u.IsGenericType && typeof(IConfigurableOptions).IsAssignableFrom(u.GetGenericTypeDefinition()));
            if (validateInterface != null)
            {
                var genericArguments = validateInterface.GenericTypeArguments;

                // 配置复杂验证
                if (genericArguments.Length > 1)
                {
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IValidateOptions<TOptions>), genericArguments.Last()));
                }

                // 配置后期配置
                var postConfigureMethod = optionsType.GetMethod(nameof(IConfigurableOptions<TOptions>.PostConfigure));
                if (postConfigureMethod != null)
                {
                    if (optionsSettings?.PostConfigureAll != true)
                        optionsConfigure.PostConfigure(options => postConfigureMethod.Invoke(options, new object[] { options, optionsConfiguration }));
                    else
                        services.PostConfigureAll<TOptions>(options => postConfigureMethod.Invoke(options, new object[] { options, optionsConfiguration }));
                }
            }

            return services;
        }

        /// <summary>
        /// 获取配置路径
        /// </summary>
        /// <param name="optionsSettings">配置特性</param>
        /// <param name="optionsType">配置类型</param>
        /// <returns></returns>
        private static string GetConfigurationPath(ConfigurableOptionsAttribute optionsSettings, Type optionsType)
        {
            // 默认后缀：Options
            string defaultStuffx = nameof(Options);

            if (optionsSettings != null)
            {
                if (!string.IsNullOrWhiteSpace(optionsSettings.Path))
                {
                    return optionsSettings.Path;
                }
            }

            return optionsType.Name.Substring(0, optionsType.Name.Length - defaultStuffx.Length);//.AsSpan().Slice(0, optionsType.Name.Length - defaultStuffx.Length).ToString();
        }
    }
}
