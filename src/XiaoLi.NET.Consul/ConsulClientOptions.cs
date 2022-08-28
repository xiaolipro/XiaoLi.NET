using System;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Configuration;

namespace XiaoLi.NET.Consul
{
    /// <summary>
    /// Consul客户端连接配置
    /// </summary>
    public class ConsulClientOptions:IAutoValidateOptions<ConsulClientOptions>
    {
        /// <summary>
        /// Consul地址
        /// </summary>
        public Uri Address { get; set; } = new Uri("http://localhost:8500");

        /// <summary>
        /// 数据中心
        /// </summary>
        public string Datacenter { get; set; } = "dc1";

        /// <summary>
        /// 配置文件名称（完整的Key）
        /// </summary>
        public string ConfigFileName { get; set; }

        public ValidateOptionsResult Validate(string name, ConsulClientOptions options)
        {
            if (options.Address == default) return ValidateOptionsResult.Fail("consul地址为空");

            return ValidateOptionsResult.Success;
        }
    }
}
