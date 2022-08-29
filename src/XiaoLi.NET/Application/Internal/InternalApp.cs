using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application.Startup;

namespace XiaoLi.NET.Application.Internal
{
    internal static class InternalApp
    {
        internal static IHostEnvironment HostEnvironment;
        internal static IConfiguration Configuration;
        internal static IServiceCollection Services;
        internal static IServiceProvider ServiceProvider;

        private static IEnumerable<Type> _startups = App.PublicTypes
            .Where(x => typeof(IAutoStartup).IsAssignableFrom(x) && !x.IsAbstract).OrderByDescending(x =>
                x.IsDefined(typeof(StartupOrderAttribute), false)
                    ? 0
                    : x.GetCustomAttribute<StartupOrderAttribute>(false).Order);

        internal static void AddJsonFiles(IConfigurationBuilder configurationBuilder)
        {
            string[] excludePrefixes = new[]
            {
                /*
                 * 默认被加载
                 * appsettings.json
                 * appsettings.{env.EnvironmentName}.json
                 */
                "appsettings"
            };

            string[] excludeSuffixes = new[]
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
                .Where(file => !prefixes.Any(file.StartsWith))
                // 过滤掉指定后缀的json文件
                .Where(file => !suffixes.Any(file.EndsWith));

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


        internal static void AddStartups(this IServiceCollection services)
        {
            foreach (var startup in _startups)
            {
                // 公开的实例成员
                var configureServicesMethods = startup.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    // 返回void且第一个参数是IServiceCollection
                    .Where(method =>
                    {
                        if (method.ReturnType != typeof(void)) return false;
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0) return false;
                        return parameters.First().ParameterType == typeof(IApplicationBuilder);
                    });

                // 调用Configure
                foreach (var configureServices in configureServicesMethods)
                {
                    configureServices.Invoke(startup, new object[] { services });
                }
            }
        }

        internal static void UseStartups()
        {
            foreach (var startup in _startups)
            {
                // 公开的实例成员
                var configureMethods = startup.GetMethods(BindingFlags.Public | BindingFlags.Instance)
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
                        .Select(parameter => ServiceProvider.GetRequiredService(parameter.ParameterType));
                    configure.Invoke(startup, parameters.ToArray());
                }
            }
        }
    }
}