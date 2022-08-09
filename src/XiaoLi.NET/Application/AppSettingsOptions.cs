using System.Collections.Generic;
using System.Linq;
using XiaoLi.NET.ConfigurableOptions;

namespace XiaoLi.NET.Application
{
    public class AppSettingsOptions : IConfigurableOptions<AppSettingsOptions>
    {

        /// <summary>
        /// 排除程序集，不扫描
        /// </summary>
        public IEnumerable<string> ExcludeAssemblies { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// 支持的第三方包前缀
        /// </summary>
        public IEnumerable<string> SupportPackagePrefixes { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// 外部程序集
        /// </summary>
        public IEnumerable<string> ExternalAssemblies { get; set; } = Enumerable.Empty<string>();

        public void PostConfigure(AppSettingsOptions options)
        {
            
        }
    }
}