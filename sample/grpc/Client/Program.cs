using Client;
using Microsoft.AspNetCore.Hosting;
using Test;
using XiaoLi.NET.Consul;
using XiaoLi.NET.Consul.LoadBalancing;
using XiaoLi.NET.Grpc;
using XiaoLi.NET.LoadBalancing;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        
    })
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build();

await host.RunAsync();