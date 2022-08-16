using Client;
using Test;
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
        services.AddGrpcLoadBalancingClient<Tester.TesterClient, ConsulResolver, RandomBalancer>("");
    })
    .Build();

await host.RunAsync();