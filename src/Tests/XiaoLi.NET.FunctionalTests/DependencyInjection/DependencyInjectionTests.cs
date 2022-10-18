using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.FunctionalTests.DependencyInjection.Services;
using Xunit.Abstractions;

namespace XiaoLi.NET.FunctionalTests.DependencyInjection;

public class DependencyInjectionTests:DependencyInjectionScenarioBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DependencyInjectionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    void Get_scope_service_success()
    {
        var userService = ServiceProvider.GetService<IUserService>();
        
        Assert.NotNull(userService);
    }
}