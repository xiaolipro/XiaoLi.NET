using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.Grpc.IntegrationTests.Infrastructure.Logging;

namespace XiaoLi.NET.Grpc.IntegrationTests.Infrastructure;

public delegate void IntegrationLogMessage(LogLevel logLevel, string categoryName, EventId eventId, string message,
    Exception exception);

public class IntegrationFixture<TStartup> : IDisposable where TStartup : class
{
    private TestServer? _server;
    private IHost? _host;
    private HttpMessageHandler? _handler;
    private Action<IWebHostBuilder>? _configureWebHost;

    public event IntegrationLogMessage? LogMessage;
    public LoggerFactory LoggerFactory { get; }

    public HttpMessageHandler Handler
    {
        get
        {
            EnsureServer();
            return _handler!;
        }
    }

    public IntegrationFixture()
    {
        LoggerFactory = new LoggerFactory();
        // if (LogMessage != null) 
        LoggerFactory.AddProvider(new IntegrationLoggerProvider(((level, name, id, message, exception) =>
        {
            LogMessage?.Invoke(level, name, id, message, exception);
        })));
    }

    public void ConfigureWebHost(Action<IWebHostBuilder> configure)
    {
        _configureWebHost = configure;
    }

    private void EnsureServer()
    {
        if (_host != null) return;

        var builder = new HostBuilder()
            .ConfigureServices((context, services) => { services.AddSingleton<ILoggerFactory>(LoggerFactory); })
            .ConfigureWebHostDefaults(webhost =>
            {
                webhost.UseTestServer()
                    .UseStartup<TStartup>();

                _configureWebHost?.Invoke(webhost);
            });

        _host = builder.Start();
        _server = _host.GetTestServer();
        _handler = _server.CreateHandler();
    }

    public void Dispose()
    {
        _server?.Dispose();
        _host?.Dispose();
        _handler?.Dispose();
        //LoggerFactory.Dispose();
    }
}