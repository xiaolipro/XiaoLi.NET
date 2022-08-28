
using System.Reflection;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Configuration;

namespace XiaoLi.NET.Consul
{
    public class ConsulRegisterOptions:IAutoValidateOptions<ConsulRegisterOptions>
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
        /// Grpc端口
        /// </summary>
        public int GrpcPort { get; set; }
        /// <summary>
        /// 服务组名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 服务标签
        /// </summary>
        public string[] Tags { get; set; }
        /// <summary>
        /// 心跳检查路由
        /// </summary>
        public string HealthCheckRoute { get; set; }
        /// <summary>
        /// 心跳接口是grpc
        /// </summary>
        public bool GrpcHelthCheck { get; set; }
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

        public ValidateOptionsResult Validate(string name, ConsulRegisterOptions options)
        {
            if (options.Port == options.GrpcPort) return ValidateOptionsResult.Fail("http端口与grpc端口不能重复");
            return ValidateOptionsResult.Success;
        }
    }
}