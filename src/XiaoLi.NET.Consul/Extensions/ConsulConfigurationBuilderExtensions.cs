using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;

namespace XiaoLi.NET.Consul.Extensions
{
    public static class ConsulConfigurationBuilderExtensions
    {
        /// <summary>
        /// 加载Consul上的配置文件
        /// </summary>
        /// <param name="consulClientOptions">配置</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">未配置</exception>
        public static IConfigurationRoot AddConsulAppsettings(this IConfigurationBuilder builder, ConsulClientOptions consulClientOptions)
        {
            Uri addrUri = consulClientOptions.Address??throw new ArgumentNullException(nameof(consulClientOptions.Address));
            string configFileName = consulClientOptions.ConfigFileName ?? throw new ArgumentNullException(nameof(consulClientOptions.ConfigFileName));


            // 加载Consul上的配置文件
            builder.AddConsul(configFileName, sources =>
            {
                sources.ConsulConfigurationOptions = options =>
                {
                    options.Address = addrUri;
                    options.Datacenter = options.Datacenter;
                };
                sources.Optional = true;
                sources.ReloadOnChange = true; // hot-update
            });

            return builder.Build();
        }
    }
}
