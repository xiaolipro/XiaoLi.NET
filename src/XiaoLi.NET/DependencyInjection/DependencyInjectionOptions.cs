using System;
using System.Collections.Generic;
using System.Text;
using XiaoLi.NET.ConfigurableOptions;
using XiaoLi.NET.DependencyInjection.Enums;

namespace XiaoLi.NET.DependencyInjection
{
    /// <summary>
    /// 依赖注入配置项
    /// </summary>
    public sealed class DependencyInjectionOptions :IConfigurableOptions
    {
        public IEnumerable<ExternalInject> ExternalServices { get; set; }
    }


    public sealed class ExternalInject
    {
        /// <summary>
        /// 排序，升序
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// 接口类型，例如："程序集名称;接口类型完整名称"
        /// </summary>
        public string[] Interfaces { get; set; }

        /// <summary>
        /// 实现类类型，例如："程序集名称;实现类类型完整名称"
        /// </summary>
        public string Implements { get; set; }
        
        /// <summary>
        /// 代理类型，例如："程序集名称;代理类型完整名称"
        /// </summary>
        public string Proxy { get; set; }
        
        /// <summary>
        /// 设置true则替换之前已经注册过的服务.使用IServiceCollection的Replace扩展方法.
        /// 设置false则只注册以前未注册的服务.使用IServiceCollection的TryAdd扩展方法.
        /// </summary>
        public bool Replace { get; set; }

        /// <summary>
        /// 生命周期:Singleton,Transient或Scoped.
        /// </summary>
        public ServiceLifecycle Lifecycle { get; set; }
        
        /// <summary>
        /// 注入模式
        /// </summary>
        public RegisterPolicy Mode { get; set; }
    }
}
