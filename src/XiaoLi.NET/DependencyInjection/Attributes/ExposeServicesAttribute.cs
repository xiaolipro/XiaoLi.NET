using System;
using System.Collections.Generic;

namespace XiaoLi.NET.DependencyInjection.Attributes
{
    /// <summary>
    /// 用于控制相关类提供了什么服务
    /// </summary>
    public class ExposeServicesAttribute:Attribute
    {
        /// <summary>
        /// 实现接口
        /// </summary>
        public IEnumerable<Type> Interfaces { get; set; }

        public ExposeServicesAttribute(params Type[] interfaces)
        {
            Interfaces = interfaces;
        }
    }
}