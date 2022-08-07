using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace XiaoLi.NET.Web.HttpClientWithPolly
{
    /// <summary>
    /// Extension methods to configure an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> for <see cref="T:Microsoft.Extensions.Http.Polly" />.
    /// </summary>
    public static class HttpClientWithPollyServiceCollectionExtensions
    {
        /// <summary>
        /// 为HttpClient添加一系列Polly策略
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clientName">客户端名称</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientWithPolly(this IServiceCollection services, string clientName,
            Action<HttpClientWithPollyOptions> action)
        {
            // 创建默认配置
            var options = new HttpClientWithPollyOptions();
            // 传递引用，调用者手动赋值覆盖默认配置
            action(options);

            #region 降级，熔断，重试，超时策略

            var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();


            // 定义降级策略
            var fallbackPolicy = Policy<HttpResponseMessage>.HandleInner<Exception>()
                .FallbackAsync(options.HttpResponseMessage,
                    async res => // The action to call asynchronously before invoking the fallback delegate.
                    {
                        logger.LogWarning("{ServiceName}开始降级，异常消息：{Message}", clientName, res.Exception.Message);
                        await Task.CompletedTask;
                    });

            // 定义熔断策略
            var circuitBreakerPolicy = Policy<HttpResponseMessage>.HandleInner<Exception>()
                .CircuitBreakerAsync(
                    options
                        .CircuitBreakerOpenFailureCount, // The number of exceptions or handled results that are allowed before opening the circuit.
                    TimeSpan.FromSeconds(options
                        .CircuitBreakerDuration), // The duration the circuit will stay open before resetting.
                    (res, ts) => // Polly.CircuitBreaker.CircuitState.Open
                    {
                        logger.LogWarning("{ServiceName}已开启断路器，持续时间：{TotalSeconds}秒，异常消息：{Message}", clientName,
                            ts.TotalSeconds, res.Exception.Message);
                    },
                    () => // Polly.CircuitBreaker.CircuitState.Closed
                    {
                        logger.LogWarning("{ServiceName}已关闭断路器", clientName);
                    },
                    () => // Polly.CircuitBreaker.CircuitState.HalfOpen
                    {
                        logger.LogWarning("{ServiceName}断路器半开启", clientName);
                    });

            // 定义重试策略
            var retryPolicy = Policy<HttpResponseMessage>.HandleInner<Exception>()
                .RetryAsync(options.RetryCount);

            // 定义超时策略
            var timeoutPolicy =
                Policy.TimeoutAsync<HttpResponseMessage>(timeout: TimeSpan.FromSeconds(options.Timeout));

            #endregion

            // 配置HttpClient
            services.AddHttpClient(clientName)
                .AddPolicyHandler(fallbackPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy);

            return services;
        }
    }
}