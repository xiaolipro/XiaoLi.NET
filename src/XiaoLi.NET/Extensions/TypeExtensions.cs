using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XiaoLi.NET.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// 获取接口的所有实现类
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(this Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(
                assembly => assembly.GetTypes().Where(t => t.GetInterfaces().Contains(interfaceType))
                );
            return types;
        }
    }
}
