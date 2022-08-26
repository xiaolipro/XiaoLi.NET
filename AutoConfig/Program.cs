// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();


using XiaoLi.NET.Application.Extensions;

var host = Host.CreateDefaultBuilder(args).ConfigureApp();

host.Build().Run();