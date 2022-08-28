using XiaoLi.NET.Configuration;

namespace AutoConfig;

public class TestOptions:IAutoOptions
{
    public string[] Arr { get; set; }
}