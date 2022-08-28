using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Server.Services;
using XiaoLi.NET.Consul.Extensions;
using XiaoLi.NET.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpcServer();
builder.Services.AddSingleton<IGreeterService, GreeterService>();

var Configuration = builder.Configuration;
builder.Services.AddConsul();

builder.WebHost.ConfigureKestrel(options =>
{
    var ports = GetDefinedPorts(Configuration);
    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

});

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("ConsulRegister:GrpcPort", 5001);
    var port = config.GetValue("ConsulRegister:Port", 80);
    return (port, grpcPort);
}


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TesterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
// 心跳检测
// app.MapGet($"/hc", () => $"Healthily {DateTime.Now:yyyy-MM-dd HH:mm:ss fff}");
app.UseHealthCheckForConsul();
app.Run();