using Microsoft.Extensions.Hosting;
using XiaoLi.NET.Application;
using XiaoLi.NET.Application.Extensions;

namespace XiaoLi.NET.UnitTests;

public class AppTest
{
    [Fact]
    public void Can_Run()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureApp()
            .Build();
        host.Run();
    }
}