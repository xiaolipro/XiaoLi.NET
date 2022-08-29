// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();


using AutoConfig;
using Microsoft.Extensions.Options;
using XiaoLi.NET.Application;
using XiaoLi.NET.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.InitWebApp().ConfigureServices(services =>
{
    services.AddMvc();
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
var appOptions = app.Services.GetRequiredService<IOptions<TestValidateOptions>>();
var t = appOptions.Value;

var appOptions2 = app.Services.GetRequiredService<IOptions<TestOptions>>();
var t2 = appOptions2.Value;


var appOptions3 = app.Services.GetRequiredService<IOptions<TestPostOptions>>();
var t3 = appOptions3.Value;
app.Run();