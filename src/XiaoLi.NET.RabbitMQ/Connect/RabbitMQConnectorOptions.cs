using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoLi.NET.RabbitMQ.Connect
{
    /// <summary>
    /// RabbitMQ连接器配置
    /// </summary>
    public class RabbitMQConnectorOptions
    {

        /// <summary>
        /// 连接失败重试次数，默认5次
        /// </summary>
        public int ConnectFailureRetryCount { get; set; } = 5;

        /// <summary>
        /// 是否开启网络自动恢复，默认开启
        /// </summary>
        public bool AutomaticRecoveryEnabled { get; set; } = true;

        /// <summary>
        /// 网络自动恢复时间间隔，默认5秒
        /// </summary>
        public TimeSpan NetworkRecoveryInterval { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 客户端配置
        /// </summary>
        public RabbitMQClientOptions RabbitMQClientOptions { get; set; } = new RabbitMQClientOptions();
    }
}
