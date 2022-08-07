using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoLi.NET.App.Consul
{
    /// <summary>
    /// Consul配置
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
