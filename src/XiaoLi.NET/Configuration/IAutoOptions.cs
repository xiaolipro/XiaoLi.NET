using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace XiaoLi.NET.Configuration
{
    /// <summary>
    /// 配置项
    /// </summary>
    public interface IAutoOptions
    {
    }
    

    /// <summary>
    /// 配置项
    /// </summary>
    public interface IAutoOptions<in TOptions> : IAutoOptions where TOptions : class,IAutoOptions
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
    public interface IAutoOptions<in TOptions, TOptionsValidation> : IAutoOptions<TOptions>
        where TOptions : class, IAutoOptions
        where TOptionsValidation : IValidateOptions<TOptions>
    {
    }
}
