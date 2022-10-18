using System;
using XiaoLi.NET.DependencyInjection.Enums;

namespace XiaoLi.NET.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyInjectionAttribute:Attribute
    {
        /// <summary>
        /// 代理类型
        /// </summary>
        public Type Proxy { get; set; }
        
        /// <summary>
        /// 设置true则替换之前已经注册过的服务.使用IServiceCollection的Replace扩展方法.
        /// 设置false则只注册以前未注册的服务.使用IServiceCollection的TryAdd扩展方法.
        /// </summary>
        public bool ReplaceService { get; set; }

        /// <summary>
        /// 生命周期:Singleton,Transient或Scoped.
        /// </summary>
        public ServiceLifecycle Lifecycle { get; set; }

        /// <summary>
        /// 注册策略
        /// </summary>
        /// <remarks>
        /// <para>如果你未指定要公开的服务ExposeServicesAttribute,则依照约定公开服务:</para>
        /// <para>默认情况下,类本身是公开的.这意味着你可以按类名注入它.</para>
        /// <para>默认情况下,默认接口是公开的.默认接口是由命名约定确定.例如A:IA,IA2,那么IA是A的默认接口,IA2不是</para>
        /// </remarks>
        public RegisterPolicy RegisterPolicy { get; set; }

        public DependencyInjectionAttribute() : this(ServiceLifecycle.Transient)
        {
            
        }

        public DependencyInjectionAttribute(ServiceLifecycle lifecycle) : this(lifecycle, RegisterPolicy.NamingConventionsInterface)
        {
            
        }
        
        public DependencyInjectionAttribute(ServiceLifecycle lifecycle,RegisterPolicy mode) : this(lifecycle,mode, true)
        {
            
        }

        public DependencyInjectionAttribute(ServiceLifecycle lifecycle, RegisterPolicy registerPolicy, bool replaceService)
        {
            Lifecycle = lifecycle;
            RegisterPolicy = registerPolicy;
            ReplaceService = replaceService;
        }
    }
}