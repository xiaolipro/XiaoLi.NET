using Client;
using Microsoft.AspNetCore.Hosting;
using Test;
using XiaoLi.NET.Consul;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.Grpc;
using XiaoLi.NET.LoadBalancing;
using XiaoLi.NET.LoadBalancing.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IGreetRepository, GreetRepository>();
        services.Configure<ConsulClientOptions>(hostContext.Configuration.GetSection("ConsulClient"));
        services.AddLoadBalancer<ConsulGrpcResolver, RandomBalancer>();
        services.AddGrpcLoadBalancingClient<Tester.TesterClient>("TestService");
    })
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build();

await host.RunAsync();