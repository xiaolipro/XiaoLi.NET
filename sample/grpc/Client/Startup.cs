using Microsoft.AspNetCore.Builder;
using Test;
using XiaoLi.NET.Consul;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.Grpc;
using XiaoLi.NET.LoadBalancing;

namespace Client;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        });
        
        
        services.AddHostedService<Worker>();
        services.AddSingleton<IGreetRepository, GreetRepository>();
        services.Configure<ConsulClientOptions>(Configuration.GetSection("ConsulClient"));
        services.AddGrpcLoadBalancingClient<Tester.TesterClient, ConsulResolver, RandomBalancer>("TestService");
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}