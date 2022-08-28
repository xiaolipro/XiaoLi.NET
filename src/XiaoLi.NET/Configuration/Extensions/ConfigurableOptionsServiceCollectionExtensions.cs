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
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            
            var types = App.PublicTypes
                .Where(type => typeof(IAutoOptions).IsAssignableFrom(type) && !type.GetTypeInfo().IsAbstract);
            foreach (var optionsType in types)
            {
                #region IConfigureOptions 自动注入

                // 获取配置路径
                string path = App.GetConfigurationPath(optionsType);

                var section = App.Configuration.GetSection(path);

                typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                        new[] { typeof(IServiceCollection), typeof(IConfiguration) })
                    ?.MakeGenericMethod(optionsType)
                    // 静态方法忽略obj参数
                    .Invoke(null, new object[] { services, section });

                #endregion

                #region IPostConfigureOptions 配置后
                
                if (typeof(IAutoPostOptions<>).MakeGenericType(optionsType).IsAssignableFrom(optionsType))
                {
                    services.TryAddSingleton(typeof(IPostConfigureOptions<>).MakeGenericType(optionsType), optionsType);
                }
                #endregion

                #region IValidateOptions 带验证
                
                if (typeof(IAutoValidateOptions<>).IsAssignableFrom(optionsType))
                {
                     services.TryAddSingleton(typeof(IValidateOptions<>).MakeGenericType(optionsType), optionsType);
                }
                #endregion
            }

            return services;
        }
        
    }
}