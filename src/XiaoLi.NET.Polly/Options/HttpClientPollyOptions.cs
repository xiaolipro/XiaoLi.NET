using System.Net.Http;

namespace XiaoLi.NET.Polly.Options
{
    /// <summary>
    /// 熔断，降级，重试，超时HttpClientPollyOptions配置类
    /// </summary>
    public sealed class HttpClientPollyOptions
    {
        /// <summary>
        /// 超时时间设置，单位为秒
        /// </summary>
        public int TimeoutTime { set; get; }

        /// <summary>
        /// 失败重试次数
        /// </summary>
        public int RetryCount { set; get; }

        /// <summary>
        /// 执行多少次异常，开启短路器（例：失败2次，开启断路器）
        /// </summary>
        public int CircuitBreakerOpenFallCount { set; get; }

        /// <summary>
        /// 断路器开启的时间(例如：设置为2秒，短路器两秒后自动由开启到关闭)
        /// </summary>
        public int CircuitBreakerDownTime { set; get; }

        /// <summary>
        /// 降级处理(将异常消息封装成为正常消息返回，然后进行响应处理，例如：系统正在繁忙，请稍后处理.....)
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { set; get; }
    }
}