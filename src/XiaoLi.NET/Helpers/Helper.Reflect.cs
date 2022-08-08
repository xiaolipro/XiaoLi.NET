using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace XiaoLi.NET.Helpers
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public partial class Helper
    {
        /// <summary>
        /// 根据物理路径加载程序集，并将其返回
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Assembly LoadAssembly(string path)
        {
            if (!File.Exists(path)) return default;
            return Assembly.LoadFrom(path);
        }
    }
}
