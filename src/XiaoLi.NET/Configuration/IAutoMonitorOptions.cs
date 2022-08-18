using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoLi.NET.Configuration
{
	/// <summary>
    /// 可热更新的配置项
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public interface IAutoMonitorOptions<TOptions> : IAutoOptions<TOptions>
        where TOptions : class, IAutoOptions
    {
        /// <summary>
        /// 配置文件发生变化时
        /// </summary>
        /// <param name="options"></param>
        void OnChange(TOptions options);
    }
}
