using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XiaoLi.NET.App.Extensions
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
            if (!interfaceType.IsInterface) return null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(
                assembly => assembly.GetTypes().Where(t => t.GetInterfaces().Contains(interfaceType))
                );
            return types;
        }


        
        /// <summary>
        /// 获取类型名称
        /// 泛型 返回 泛型参数Name以separator拼接
        /// 非泛型 直接返回 Name
        /// </summary>
        /// <param name="type"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetTypeName(this Type type,string separator = ",")
        {
            // 不是泛型直接返回
            if (!type.IsGenericType) return type.Name;

            var genericTypes = string.Join(separator, type.GetGenericArguments().Select(x => x.Name).ToArray());
            
            // <A,B>
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
    }
}
