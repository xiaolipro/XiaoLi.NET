using Client;
using Microsoft.AspNetCore.Hosting;
using Test;
using XiaoLi.NET.Application.Extensions;
using XiaoLi.NET.Consul;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.Grpc;
using XiaoLi.NET.LoadBalancing;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IGreetRepository, GreetRepository>();
        services.Configure<ConsulClientOptions>(hostContext.Configuration.GetSection("ConsulClient"));
        services.AddGrpcClientLoadBalancer<ConsulGrpcResolver, RandomBalancer>();
        services.AddGrpcLoadBalancingClient<Tester.TesterClient>("TestService");
    })
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build();

await host.RunAsync();