using System;
using System.Collections.Generic;
using System.Text;

namespace XiaoLi.NET.App.ConfigurableOptions
{
	/// <summary>
    /// 可热更新的配置项
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public interface IConfigurableMonitorOptions<TOptions> : IConfigurableOptions<TOptions>
        where TOptions : class, IConfigurableOptions
    {
        /// <summary>
        /// 配置文件发生变化时
        /// </summary>
        /// <param name="options"></param>
        void OnChange(TOptions options);
    }
}
