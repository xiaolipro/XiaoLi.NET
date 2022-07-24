using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoLi.RabbitMQ.Configs
{
    public class RabbitMQConfig
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

        /// <summary>
        /// 重连次数
        /// </summary>
        public int Retries { get; set; }
    }
}
