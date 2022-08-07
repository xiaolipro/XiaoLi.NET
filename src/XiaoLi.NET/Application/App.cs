using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace XiaoLi.NET.App.Application
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public static class App
    {
        /// <summary>
        /// 配置项
        /// </summary>
        public static IConfiguration Configuration => InternalApp.Configuration;
    }
}
