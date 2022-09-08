using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace XiaoLi.NET.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="app"></param>
        /// <param name="healthCheckRoute">路由</param>
        /// <returns></returns>
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string healthCheckRoute = "/hc")
        {
            app.Map($"/{healthCheckRoute.Trim('/')}", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync($"Healthy {DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
                });
            });

            return app;
        }
    }
}