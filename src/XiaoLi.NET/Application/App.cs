using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Application.Internal;
using XiaoLi.NET.Configuration.Attributes;
using XiaoLi.NET.Helpers;

namespace XiaoLi.NET.Application
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public static class App
    {
        /// <summary>
        /// 基础配置AppSettings
        /// </summary>
        public static AppOptions Settings => GetConfiguration<AppOptions>("AppSettings");

        /// <summary>
        /// 获取主机环境
        /// </summary>
        public static IHostEnvironment HostEnvironment => InternalApp.HostEnvironment;

        /// <summary>
        /// 所有配置
        /// </summary>
        public static IConfiguration Configuration => InternalApp.Configuration;

        /// <summary>
        /// 所有程序集
        /// </summary>
        public static readonly IEnumerable<Assembly> Assemblies;

        /// <summary>
        /// 外部程序集
        /// </summary>
        public static readonly IEnumerable<Assembly> ExternalAssemblies;

        /// <summary>
        /// 所有公开的类型
        /// </summary>
        public static readonly IEnumerable<Type> PublicTypes;

        /// <summary>
        /// 获取配置，找不到会默认调用并返回new()
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public static TOptions GetConfiguration<TOptions>(string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = GetConfigurationPath<TOptions>();
            }
            var options = Configuration.GetSection(path).Get<TOptions>();

            if (options == null) options = Activator.CreateInstance<TOptions>();

            if (typeof(IConfiguration).IsAssignableFrom(typeof(TOptions)))
            {
                typeof(TOptions).GetMethod("PostConfigure")?.Invoke(options, new object[] { options });
            }

            return options;
        }

        /// <summary>
        /// 获取配置路径
        /// </summary>
        /// <returns></returns>
        internal static string GetConfigurationPath<TOptions>()
        {
            var optionsType = typeof(TOptions);
            return GetConfigurationPath(optionsType);
        }
        
        internal static string GetConfigurationPath(Type optionsType)
        {
            var attr = optionsType.GetCustomAttribute<PathAttribute>(false);

            if (attr != null)
            {
                if (!string.IsNullOrWhiteSpace(attr.Path))
                {
                    return attr.Path;
                }
            }

            // 默认后缀：Options
            string defaultStuffx = nameof(Options);

            // 切除后缀
            //.AsSpan().Slice(0, optionsType.Name.Length - defaultStuffx.Length).ToString();
            return optionsType.Name.Substring(0, optionsType.Name.Length - defaultStuffx.Length);
        }


        static App()
        {
            // 解析程序集
            var (assemblies, externalAssemblies) = ResolveAssemblies();
            Assemblies = assemblies;
            ExternalAssemblies = externalAssemblies;
            // 解析程序集所有public类型
            PublicTypes = assemblies.SelectMany(x => x.GetTypes()).Where(x => x.IsPublic);
        }


        private static (IEnumerable<Assembly> assemblies, IEnumerable<Assembly> externalAssemblies) ResolveAssemblies()
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
                })

                .Select(lib => Assembly.Load(lib.Name));
            // 加载外部程序集
            var externalAssemblies = Settings.ExternalAssemblies
                .Select(externalAssembly
                    => Path.Combine(AppContext.BaseDirectory,
                        externalAssembly.EndsWith(".dll") ? externalAssembly : externalAssembly + ".dll"))
                .Select(Helper.LoadAssemblyByPath)
                .Where(assembly => assembly != default);


            return (assemblies, externalAssemblies);
        }



    }
}