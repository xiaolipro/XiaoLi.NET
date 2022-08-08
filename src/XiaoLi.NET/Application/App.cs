using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.ConfigurableOptions;
using XiaoLi.NET.Helpers;

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


        static InternalApp()
        {
            // 解析环境变量
            ResolveEnvironmentVariables();
        }


        internal static void ConfigureApp(IHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration(((context, configurationBuilder) =>
                {
                    HostingEnvironment = context.HostingEnvironment;
                    // 添加json配置文件
                    AddJsonFiles(configurationBuilder);
                }))
                .ConfigureServices(((context, services) => { Configuration = context.Configuration; }));
        }

        static void AddJsonFiles(IConfigurationBuilder configurationBuilder)
        {
            string executeDir = AppContext.BaseDirectory; //程序执行目录

            // 获取执行目录下所有json文件，TopDirectoryOnly不递归
            var jsonFiles = Directory.GetFiles(executeDir, "*.json", SearchOption.TopDirectoryOnly);

            // jsonFiles.Where(file => )
            foreach (var file in jsonFiles)
            {
                if (VaildateJsonFile(file))
                {
                    configurationBuilder.AddJsonFile(file, true, true);
                }
            }
        }

        static bool VaildateJsonFile(string file)
        {
            var arr = file.Split('.');
            if (arr.Length == 2) return true;
            if (arr.Any(x => string.IsNullOrWhiteSpace(x))) return false;
            return file.EndsWith($".{HostingEnvironment.EnvironmentName}.json");
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