using Microsoft.Extensions.Options;

namespace XiaoLi.NET.Configuration
{
    /// <summary>
    /// 自动校验Options
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public interface IAutoValidateOptions<TOptions> : IAutoOptions, IValidateOptions<TOptions> where TOptions : class
    {
    }
}