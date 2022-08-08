using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.ConfigurableOptions;
using XiaoLi.NET.Helpers;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace XiaoLi.NET.Application
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public static class App
    {
        /// <summary>
        /// 应用程序全局配置
        /// </summary>
        public static AppSettingsOptions Settings => GetConfiguration<AppSettingsOptions>("AppSettings");

        /// <summary>
        /// 获取宿主机环境
        /// </summary>
        public static IHostingEnvironment HostEnvironment => InternalApp.HostingEnvironment;

        /// <summary>
        /// 所有配置
        /// </summary>
        public static IConfiguration Configuration => InternalApp.Configuration;


        internal static IEnumerable<Assembly> Assemblies;
        internal static IEnumerable<Assembly> ExternalAssemblies;

        /// <summary>
        /// 所有公开的类型
        /// </summary>
        public static readonly IEnumerable<Type> PublicTypes;

        public static TOptions GetConfiguration<TOptions>(string path) =>
            Configuration.GetSection(path).Get<TOptions>();

        static App()
        {
            // 解析程序集
            ResolveAssemblies();
            // 解析类型
            PublicTypes = Assemblies.SelectMany(x => x.GetTypes()).Where(x => x.IsPublic);
        }

        private static void ResolveAssemblies()
        {
            // 扫描项目程序集
            var assemblies = DependencyContext.Default.RuntimeLibraries
                .Where(lib =>
                {
                    if (lib.Type.Equals("project") && !Settings.ExcludeAssemblies.Any(x => lib.Name.EndsWith(x)))
                        return true;
                    if (lib.Type.Equals("package"))
                    {
                        if (lib.Name.StartsWith(nameof(XiaoLi))) return true;
                        if (Settings.SupportPackagePrefixes.Any(x => lib.Name.StartsWith(x))) return true;
                    }

                    return false;
                });


            var externalAssemblies = Enumerable.Empty<Assembly>().ToList();

            // 加载外部程序集
            foreach (var externalAssembly in Settings.ExternalAssemblies)
            {
                // 物理路径
                string path = Path.Combine(AppContext.BaseDirectory,
                    externalAssembly.EndsWith(".dll") ? externalAssembly : externalAssembly + ".dll");

                // 加载
                var assembly = Helper.LoadAssembly(path);
                if (assembly == default) continue;

                externalAssemblies.Add(assembly);
            }

            // 合并
            Assemblies = Assemblies.Concat(externalAssemblies);
            ExternalAssemblies = externalAssemblies;
        }
    }


    internal static class InternalApp
    {
        internal static IHostingEnvironment HostingEnvironment;
        internal static IConfiguration Configuration;
        internal static IServiceCollection Services;
        internal static IServiceProvider ServiceProvider;
        internal static List<IStartup> Startups;

        static InternalApp()
        {
            // 解析环境变量
            ResolveEnvironmentVariables();
        }



        internal static void Configure(IApplicationBuilder app)
        {
            ServiceProvider = app.ApplicationServices;
            UseStartups(app);
        }

        private static void UseStartups(this IApplicationBuilder app)
        {
            foreach (var startup in InternalApp.Startups)
            {
                // startup.Configure(app);

                var startupType = startup.GetType();

                // 公开的实例成员
                var configureMethods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    // 返回void且第一个参数是IApplicationBuilder方法
                    .Where(method =>
                    {
                        if (method.ReturnType != typeof(void)) return false;
                        var parameters = method.GetParameters();
                        if (parameters.Length < 1) return false;
                        // if (parameters.Length == 1 && method.Name.Equals(nameof(Configure))) return false;
                        return parameters.First().ParameterType == typeof(IApplicationBuilder);
                    });

                // 调用Configure
                foreach (var configure in configureMethods)
                {
                    var parameters = configure.GetParameters()
                        .Select(parameter => InternalApp.ServiceProvider.GetRequiredService(parameter.ParameterType));
                    configure.Invoke(startup, parameters.ToArray());
                }
            }
        }

        static void ResolveEnvironmentVariables()
        {
            string env = HostingEnvironment.EnvironmentName;
            if (string.IsNullOrWhiteSpace(env))
            {
                env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

                if (string.IsNullOrWhiteSpace(env)) throw new Exception("无法解析当前环境变量，请检查NETCORE_ENVIRONMENT");

                HostingEnvironment.EnvironmentName = env;
            }
        }
    }
}