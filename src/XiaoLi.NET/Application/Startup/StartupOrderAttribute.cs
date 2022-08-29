using System;

namespace XiaoLi.NET.Application.Startup
{
    /// <summary>
    /// 启动顺序，降序
    /// </summary>
    public class StartupOrderAttribute:Attribute
    {
        public int Order { get; set; }

        public StartupOrderAttribute(int order)
        {
            Order = order;
        }
    }
}