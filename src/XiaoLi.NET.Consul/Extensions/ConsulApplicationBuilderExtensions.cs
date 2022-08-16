using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var consulRegisterOptions =
                app.ApplicationServices.GetRequiredService<IOptions<ConsulRegisterOptions>>().Value ??
                throw new ArgumentNullException(nameof(ConsulRegisterOptions));
            
            // 心跳检测
            app.Map($"/{consulRegisterOptions.HealthCheckRoute.Trim('/')}", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(
                        $"{consulRegisterOptions.ServiceName} {DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
                });
            });

            return app;
        }
    }
}