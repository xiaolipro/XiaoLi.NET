using System.ComponentModel;

namespace XiaoLi.NET.DependencyInjection.Enums
{
    public enum RegisterPolicy
    {
        [Description("仅实现")]
        OnlyImplement,
        [Description("实现第一个接口")]
        FirstInterface,
        [Description("按命名约定的接口")]
        NamingConventionsInterface,
        [Description("实现所有接口")]
        AllInterfaces
    }
}