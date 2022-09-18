using System;

namespace XiaoLi.NET.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SuppressUnifiedResultAttribute:Attribute
    {
        /*
         * 被标记的控制器/方法，将不再被UnifiedResultFilter处理
         */
    }
}