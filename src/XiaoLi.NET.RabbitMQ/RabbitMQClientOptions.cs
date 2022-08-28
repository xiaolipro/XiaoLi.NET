using System;
using System.Collections.Generic;
using System.Text;
using XiaoLi.NET.Configuration;

namespace XiaoLi.NET.RabbitMQ
{
    public class RabbitMQClientOptions:IAutoOptions
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string VirtualHost { get; set; }
    }
}
