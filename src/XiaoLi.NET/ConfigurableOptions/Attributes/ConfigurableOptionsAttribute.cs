using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XiaoLi.NET.ConfigurableOptions.Attributes
{
    /// <summary>
    /// 配置项特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigurableOptionsAttribute : Attribute
    {
        /// <summary>
        /// 对应配置文件中的路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">配置文件对应节点的路径</param>
        public ConfigurableOptionsAttribute(string path)
        {
            Path = path;
        }
    }
}
