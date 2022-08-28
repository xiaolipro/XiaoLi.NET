using Microsoft.Extensions.Options;

namespace XiaoLi.NET.Configuration
{
    /// <summary>
    /// 从DI容器中解析Options
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public interface IAutoPostOptions<TOptions> : IAutoOptions, IPostConfigureOptions<TOptions> where TOptions : class,IAutoOptions
    {
    }
}