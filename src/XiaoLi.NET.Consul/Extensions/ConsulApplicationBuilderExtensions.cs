using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Consul.Register;

namespace XiaoLi.NET.Consul.Extensions
{
    public static class ConsulApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用Consul注册机，将服务注册到Consul
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task UseConsulRegistry(this IApplicationBuilder app)
        {
            var consulRegister = app.ApplicationServices.GetRequiredService<IConsulRegister>();
            await consulRegister.Registry();
        }
    }
}
