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
        userService.Increment();
        userService.Increment();
        _testOutputHelper.WriteLine(userService.GetV().ToString());
        
        var userService2 = ServiceProvider.GetService<UserService>();
        Assert.NotNull(userService2);
        userService2.Increment();
        _testOutputHelper.WriteLine(userService2.GetV().ToString());
    }
    
    [Fact]
    void Get_scope_imp_success()
    {
        var userService = ServiceProvider.GetService<UserService>();
        
        Assert.NotNull(userService);
    }

    [Fact]
    async Task Get_user_info_by_id()
    {
        var userService = ServiceProvider.GetService<IUserService>();
        var user = await userService!.GetUserById(1);
        
        Assert.Equal(1,user.Id);
        Assert.Equal("用户1",user.Name);
        Assert.True(user.Age >= 18);
    }


    [Fact]
    void Generic_class_can_register()
    {
        var res = ServiceProvider.GetService<IRepository<string>>();
        res.Count();
    }

    [Fact]
    void Mutil_impl()
    {
        var userService = base.ServiceProvider.GetService<IUserService>();
        userService!.Increment();
    }
}