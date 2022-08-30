using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Application;
using XiaoLi.NET.Application.Internal;
using XiaoLi.NET.Startup.Attributes;

namespace XiaoLi.NET.Startup.Extensions
{
    public static class StartupServiceCollenctionExtensions
    {
        
        /// <summary>
        /// 调用所有IAutoStart 公开的实例成员中返回void且第一个参数是IServiceCollection的方法
        /// </summary>
        /// <param name="services"></param>
        internal static void AddStartups(this IServiceCollection services)
        {
            foreach (var startup in InternalApp.Startups)
            {
                // 公开的实例成员
                var configureServicesMethods = startup.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    // 返回void且第一个参数是IServiceCollection
                    .Where(method =>
                    {
                        if (method.ReturnType != typeof(void)) return false;
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0) return false;
                        return parameters.First().ParameterType == typeof(IServiceCollection);
                    });

                // 调用ConfigureServices
                foreach (var configureServices in configureServicesMethods)
                {
                    configureServices.Invoke(startup, new object[] { services });
                }
            }
        }
    }
}