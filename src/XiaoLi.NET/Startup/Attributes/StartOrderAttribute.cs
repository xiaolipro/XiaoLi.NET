using System;

namespace XiaoLi.NET.Startup.Attributes
{
    /// <summary>
    /// 启动顺序，降序
    /// </summary>
    public class StartOrderAttribute:Attribute
    {
        public int Order { get; set; }

        public StartOrderAttribute(int order)
        {
            Order = order;
        }
    }
}