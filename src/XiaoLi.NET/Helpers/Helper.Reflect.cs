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
        /// 根据物理路径加载程序集
        /// </summary>
        /// <param name="path">物理路径</param>
        /// <returns>程序集</returns>
        public static Assembly LoadAssemblyByPath(string path)
        {
            return !File.Exists(path) ? default : Assembly.LoadFrom(path);
        }
    }
}
