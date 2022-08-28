using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Application;

namespace XiaoLi.NET.Consul.Extensions
{
    public static class ConsulApplicationBuilderExtensions
    {
        /// <summary>
        /// 添加Consul2Server心跳检测
        /// </summary>
        /// <remarks>
        /// 当然你也可以自己实现，不过心跳地址一定要和ConsulRegister:HealthCheckRoute保持一致
        /// </remarks>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthCheckForConsul(this IApplicationBuilder app)
        {
            var consulRegisterOptions = App.GetConfiguration<ConsulRegisterOptions>();

            // 心跳检测
            app.Map($"/{consulRegisterOptions.HealthCheckRoute.Trim('/')}", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync(
                        $"{consulRegisterOptions.ServiceName} {DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
                });
            });

            return app;
        }
    }
}