using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Hosting;
using XiaoLi.NET.Startup;

namespace XiaoLi.NET.Application.Internal
{
    internal static class InternalApp
    {
        internal static IHostEnvironment HostEnvironment;
        internal static IConfiguration Configuration;
        internal static IServiceProvider ServiceProvider;

        internal static IEnumerable<IAutoStart> Startups;

        internal static void AddJsonFiles(IConfigurationBuilder configurationBuilder)
        {
            string[] excludePrefixes = {
                /*
                 * 默认被加载
                 * appsettings.json
                 * appsettings.{env.EnvironmentName}.json
                 */
                "appsettings"
            };

            string[] excludeSuffixes = {
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

        internal static IHostEnvironment ResolveEnvironmentVariables(IHostEnvironment hostEnvironment)
        {
            string env = hostEnvironment.EnvironmentName;
            if (string.IsNullOrWhiteSpace(env))
            {
                env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

                if (string.IsNullOrWhiteSpace(env)) throw new Exception("无法解析当前环境变量，请检查NETCORE_ENVIRONMENT");

                hostEnvironment.EnvironmentName = env;
            }

            return hostEnvironment;
        }


        private static bool VaildateJsonFile(string file)
        {
            var arr = file.Split('.');

            if (arr.Length == 2) return true;
            if (arr.Any(string.IsNullOrWhiteSpace)) return false;

            return file.EndsWith($".{HostEnvironment.EnvironmentName}.json");
        }


        

        
    }
}