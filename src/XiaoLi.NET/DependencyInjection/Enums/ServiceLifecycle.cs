using System.ComponentModel;

namespace XiaoLi.NET.DependencyInjection.Enums
{
    /// <summary>
    /// 服务的生命周期
    /// </summary>
    public enum ServiceLifecycle
    {
        [Description("瞬态")]
        Transient,
        [Description("局域")]
        Scoped,
        [Description("单例")]
        Singleton
    }
}