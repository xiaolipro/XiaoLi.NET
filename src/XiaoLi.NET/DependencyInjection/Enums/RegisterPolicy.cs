using System.ComponentModel;

namespace XiaoLi.NET.DependencyInjection.Enums
{
    public enum RegisterPolicy
    {
        [Description("仅实现")]
        OnlyImplement,
        [Description("实现第一个接口")]
        FirstInterface,
        [Description("命名约定")]
        NamingConventions,
        [Description("实现所有接口")]
        AllInterfaces
    }
}