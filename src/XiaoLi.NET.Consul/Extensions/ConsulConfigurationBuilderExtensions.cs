using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Winton.Extensions.Configuration.Consul;
using XiaoLi.NET.Application;

namespace XiaoLi.NET.Consul.Extensions
{
    public static class ConsulConfigurationBuilderExtensions
    {
        /// <summary>
        /// 加载Consul上的配置文件
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">未配置</exception>
        public static IConfigurationRoot AddConsulConfiguration(this IConfigurationBuilder builder)
        {
            var consulClientOptions = App.GetOptions<ConsulClientOptions>();
            
            // 加载Consul上的配置文件
            builder.AddConsul(consulClientOptions.ConfigFileName, sources =>
            {
                sources.ConsulConfigurationOptions = options =>
                {
                    options.Address = consulClientOptions.Address;
                    options.Datacenter = consulClientOptions.Datacenter;
                };
                sources.Optional = true;
                sources.ReloadOnChange = true; // hot-update
            });

            return builder.Build();
        }
    }
}
