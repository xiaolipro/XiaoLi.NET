using Microsoft.Extensions.Options;
using XiaoLi.NET.Configuration;

namespace AutoConfig;

public class TestValidateOptions:IAutoValidateOptions<TestValidateOptions>
{
    public string A { get; set; }
    public int B { get; set; }
    public ValidateOptionsResult Validate(string name, TestValidateOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.A))
        {
            return ValidateOptionsResult.Fail("A不能为空");
        }
        
        return ValidateOptionsResult.Success;
    }
}