using System;
using System.Net.Http;

namespace XiaoLi.NET.App.HttpClientWithPolly
{
    /// <summary>
    /// HttpClient降级，熔断，重试，超时配置类
    /// </summary>
    public sealed class HttpClientWithPollyOptions : IDisposable
    {
        /// <summary>
        /// 超时时间设置，默认3秒
        /// </summary>
        public int Timeout { set; get; } = 3;

        /// <summary>
        /// 失败重试次数，默认3次
        /// </summary>
        public int RetryCount { set; get; } = 3;

        /// <summary>
        /// 执行多少次异常后，开启断路器，默认3次
        /// </summary>
        public int CircuitBreakerOpenFailureCount { set; get; } = 3;

        /// <summary>
        /// 断路持续的时间，默认60秒
        /// 例如：设置为2秒，短路器两秒后自动由开启到关闭
        /// </summary>
        public int CircuitBreakerDuration { set; get; } = 60;

        /// <summary>
        /// 降级处理.
        /// 将异常消息封装成为正常消息返回，然后进行响应处理，例如：系统正在繁忙，请稍后处理.....
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { set; get; }

        public HttpClientWithPollyOptions()
        {
            HttpResponseMessage = new HttpResponseMessage();
        }
        
        public void Dispose()
        {
            HttpResponseMessage?.Dispose();
        }
    }
}