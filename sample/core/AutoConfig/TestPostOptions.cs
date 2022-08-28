using XiaoLi.NET.Configuration;

namespace AutoConfig;

public class TestPostOptions:IAutoPostOptions<TestPostOptions>
{
    public string A { get; set; }
    public void PostConfigure(string name, TestPostOptions options)
    {
        Console.WriteLine("hello");
    }
}