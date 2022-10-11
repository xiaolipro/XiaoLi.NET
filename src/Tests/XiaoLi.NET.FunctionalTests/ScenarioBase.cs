using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XiaoLi.NET.FunctionalTests;


/// <summary>
/// 脚本基类
/// </summary>
/// <typeparam name="TStartup"></typeparam>
public class ScenarioBase<TStartup> :IDisposable where TStartup : class
{
    protected TestServer Server { get; }

    protected HttpClient Client { get; }
    
    protected IServiceProvider ServiceProvider { get; set; }

    private readonly IHost _host;

    protected ScenarioBase()
    {
        var builder = CreateHostBuilder();

        _host = builder.Build();
        _host.Start();

        Server = _host.GetTestServer();
        Client = _host.GetTestClient();

        ServiceProvider = Server.Services;
    }

    protected virtual IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseStartup<TStartup>();
                webBuilder.UseTestServer();
            })
            //.UseAutofac()
            .ConfigureServices(ConfigureServices);
    }

    protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {

    }


    public void Dispose()
    {
        _host?.Dispose();
    }
}