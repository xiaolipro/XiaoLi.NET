using System;

namespace XiaoLi.NET.Consul
{
    /// <summary>
    /// Consul客户端连接配置
    /// </summary>
    public class ConsulClientOptions
    {
        /// <summary>
        /// Consul地址
        /// </summary>
        public Uri Address { get; set; }
        /// <summary>
        /// 数据中心
        /// </summary>
        public string Datacenter { get; set; }

        /// <summary>
        /// 配置文件名称（完整的Key）
        /// </summary>
        public string ConfigFileName { get; set; }
    }
}
