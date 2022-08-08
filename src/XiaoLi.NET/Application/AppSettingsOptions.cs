using System.Collections.Generic;
using System.Linq;
using XiaoLi.NET.ConfigurableOptions;

namespace XiaoLi.NET.Application
{
    public class AppSettingsOptions : IConfigurableOptions<AppSettingsOptions>
    {
        /// <summary>
        /// 配置文件扫描目录
        /// </summary>
        public IEnumerable<string> ConfigFileScanDirs { get; set; }

        /// <summary>
        /// 配置文件排除后缀
        /// </summary>
        public IEnumerable<string> ConfigFileExcludeSuffixes { get; set; }

        /// <summary>
        /// 排除程序集，不扫描
        /// </summary>
        public IEnumerable<string> ExcludeAssemblies { get; set; }

        /// <summary>
        /// 支持的第三方包前缀
        /// </summary>
        public IEnumerable<string> SupportPackagePrefixes { get; set; }

        /// <summary>
        /// 外部程序集
        /// </summary>
        public IEnumerable<string> ExternalAssemblies { get; set; }

        public void PostConfigure(AppSettingsOptions options)
        {
            ConfigFileExcludeSuffixes = ConfigFileExcludeSuffixes.Concat(new[]
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
                "runtimeconfig.json"
            });
        }
    }
}