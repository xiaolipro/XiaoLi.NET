// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();


using Microsoft.Extensions.Options;
using XiaoLi.NET.Application;
using XiaoLi.NET.Application.Extensions;

var host = Host.CreateDefaultBuilder(args).ConfigureApp();

var app = host.Build();


var appOptions = app.Services.GetRequiredService<IOptions<AppOptions>>();
var t = appOptions.Value;


var appOptions2 = app.Services.GetRequiredService<IOptions<AppOptions>>();
var t2 = appOptions.Value;
app.Run();