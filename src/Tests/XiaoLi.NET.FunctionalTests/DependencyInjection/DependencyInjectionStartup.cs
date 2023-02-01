using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.FunctionalTests.DependencyInjection.Services;

namespace XiaoLi.NET.FunctionalTests.DependencyInjection;

public class DependencyInjectionStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped(typeof(IUserService), typeof(UserService));
        services.AddScoped(typeof(IUserService), typeof(CustomUserService));
    }

    public void Configure(IApplicationBuilder app)
    {
        
    }
}