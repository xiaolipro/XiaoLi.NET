using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.ConfigurableOptions
{
    /// <summary>
    /// 配置项
    /// </summary>
    public interface IConfigurableOptions
    {
    }

    /// <summary>
    /// 配置项
    /// </summary>
    public interface IConfigurableOptions<TOptions> : IConfigurableOptions where TOptions : class,IConfigurableOptions
    {
        /// <summary>
        /// 读取完配置文件后执行
        /// </summary>
        /// <param name="options"></param>
        void PostConfigure(TOptions options);
    }

    /// <summary>
    /// 带验证的配置项
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TOptionsValidation"></typeparam>
    public interface IConfigurableOptions<TOptions, TOptionsValidation> : IConfigurableOptions<TOptions>
        where TOptions : class, IConfigurableOptions
        where TOptionsValidation : IValidateOptions<TOptions>
    {
    }
}
