using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Startup;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace XiaoLi.NET.Application.Internal
{
    internal static class InternalApp
    {
        internal static IHostEnvironment HostEnvironment;
#if NETCOREAPP3_0_OR_GREATER
        internal static IWebHostEnvironment WebHostEnvironment;
#else
        internal static IHostingEnvironment WebHostEnvironment;
#endif
        internal static IConfiguration Configuration;
        internal static IServiceProvider ServiceProvider;

        internal static IEnumerable<IAutoStart> Startups;

        internal static void AddJsonFiles(IConfigurationBuilder configurationBuilder)
        {
            string[] excludePrefixes =
            {
                /*
                 * 默认被加载
                 * appsettings.json
                 * appsettings.{env.EnvironmentName}.json
                 */
                "appsettings"
            };

            string[] excludeSuffixes =
            {
                /*
                 * 应用程序依赖关系文件
                 * When a .NET application is compiled, the SDK generates a JSON manifest file (<ApplicationName>.deps.json)
                 * that contains information about application dependencies. You can use the DependencyContext class to read
                 * information from this manifest at run time.
                 */
                "deps.json",

                /* 
                 * 运行时配置文件
                 * When a project is built, an [appname].runtimeconfig.json file is generated in the output directory.
                 * If a runtimeconfig.template.json file exists in the same folder as the project file, any configuration
                 * options it contains are inserted into the [appname].runtimeconfig.json file. If you're building the app yourself,
                 * put any configuration options in the runtimeconfig.template.json file. If you're just running the app,
                 * insert them directly into the [appname].runtimeconfig.json file.
                 */
                "runtimeconfig.json",
                "runtimeconfig.dev.json",
                "runtimeconfig.prod.json"
            };

            var configuration = configurationBuilder.Build();

            // 扫描目录
            var scanDirs = configuration.GetSection("ConfigFile:ScanDirs").Get<string[]>() ??
                           Enumerable.Empty<string>();
            scanDirs = scanDirs.Append(AppContext.BaseDirectory);

            // 过滤前缀
            var prefixes = configuration.GetSection("ConfigFile:ExcludePrefixes").Get<string[]>() ??
                           Enumerable.Empty<string>();
            prefixes = prefixes.Concat(excludePrefixes);

            // 过滤后缀
            var suffixes = configuration.GetSection("ConfigFile:ExcludeSuffixes").Get<string[]>() ??
                           Enumerable.Empty<string>();
            suffixes = suffixes.Concat(excludeSuffixes);

            // 获取目录下所有json文件，TopDirectoryOnly不递归
            var jsonFiles = scanDirs
                .SelectMany(dir => Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
                // 过滤前缀
                .Where(file => !prefixes.Any(Path.GetFileName(file).StartsWith))
                // 过滤掉指定后缀
                .Where(file => !suffixes.Any(Path.GetFileName(file).EndsWith));

            foreach (var file in jsonFiles)
            {
                if (VaildateJsonFile(file))
                {
                    configurationBuilder.AddJsonFile(file, true, true);
                }
            }
        }

#if NETCOREAPP3_0_OR_GREATER
        internal static IWebHostEnvironment ResolveWebEnvironmentVariables(IWebHostEnvironment hostEnvironment)
        {
#else
        internal static IHostingEnvironment ResolveWebEnvironmentVariables(IHostingEnvironment hostEnvironment)
        {
#endif
            hostEnvironment.EnvironmentName ??= Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "UnKnown";
            return hostEnvironment;
        }


        internal static IHostEnvironment ResolveEnvironmentVariables(IHostEnvironment hostEnvironment)
        {
            hostEnvironment.EnvironmentName ??= Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "UnKnown";
            return hostEnvironment;
        }


        private static bool VaildateJsonFile(string file)
        {
            string fileName = Path.GetFileName(file);
            var arr = fileName.Split('.');

            if (arr.Length == 2) return true;
            if (arr.Any(string.IsNullOrWhiteSpace)) return false;

            return fileName.EndsWith(
                $".{HostEnvironment?.EnvironmentName ?? WebHostEnvironment!.EnvironmentName}.json");
        }
    }
}