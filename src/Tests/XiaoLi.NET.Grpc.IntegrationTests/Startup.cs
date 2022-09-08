using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XiaoLi.NET.Grpc.IntegrationTests.Services;
using XiaoLi.NET.Grpc.Interceptors;

namespace XiaoLi.NET.Grpc.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<ServerLogInterceptor>();
        });

        services.AddSingleton<IGreeterService, GreeterService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<TesterService>();
        });
    }
}