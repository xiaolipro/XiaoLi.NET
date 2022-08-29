using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Application.Internal;

namespace XiaoLi.NET.Startup.Extensions
{
    public static class StartupApplicationBuilderExtensions
    {
        internal static void UseStartups(this IApplicationBuilder app)
        {
            foreach (var startup in InternalApp.Startups.Reverse())
            {
                // 公开的实例成员
                var configureMethods = startup.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    // 返回void且第一个参数是IApplicationBuilder
                    .Where(method =>
                    {
                        if (method.ReturnType != typeof(void)) return false;
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0) return false;
                        return parameters.First().ParameterType == typeof(IApplicationBuilder);
                    });

                // 调用Configure
                foreach (var configure in configureMethods)
                {
                    var parameters = configure.GetParameters()
                        .Where((item, idx) => idx > 0)
                        .Select(parameter => InternalApp.ServiceProvider.GetRequiredService(parameter.ParameterType));
                    configure.Invoke(startup, new []{app}.Concat(parameters).ToArray());
                }
            }
        }
    }
}