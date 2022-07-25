using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XiaoLi.NET.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetTypes(this Type @type)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(
                assembly => assembly.GetTypes().Where(t => t.GetInterfaces().Contains(@type))
                );
            return types;
        }
    }
}
