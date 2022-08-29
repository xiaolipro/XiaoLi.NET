using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using XiaoLi.NET.Startup.Extensions;

namespace XiaoLi.NET.Application.Internal
{
    /// <summary>
    /// 内部自启动中间件
    /// </summary>
    internal class InternalStartupFilter:IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                InternalApp.ServiceProvider = app.ApplicationServices;
                app.UseStartups();
                next(app);
            };
        }
    }
}