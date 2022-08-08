
using System.Reflection;

namespace XiaoLi.NET.Consul.Register
{
    public class ConsulRegisterOptions
    {
        
        /// <summary>
        /// 服务IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 服务组名称
        /// </summary>
        public string ServiceName { get; set; } = Assembly.GetEntryAssembly().GetName().Name;
        /// <summary>
        /// 服务标签
        /// </summary>
        public string[] Tags { get; set; }
        /// <summary>
        /// 心跳检查Url
        /// </summary>
        public string HealthCheckUrl { get; set; }
        /// <summary>
        /// 心跳检测间隔(s)
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 心跳超时时间(s)
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 心跳停止多久后注销服务(s)
        /// </summary>
        public int DeregisterTime { get; set; }
        /// <summary>
        /// 权重(使用权重调度器时有效)
        /// </summary>
        public int Weight { get; set; }
    }
}