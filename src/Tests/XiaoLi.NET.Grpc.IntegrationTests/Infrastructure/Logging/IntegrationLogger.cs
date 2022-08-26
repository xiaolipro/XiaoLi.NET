using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.IntegrationTests.Infrastructure.Logging;

public class IntegrationLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IntegrationLogMessage? _integrationLogMessage;

    public IntegrationLogger(string categoryName, IntegrationLogMessage? integrationLogMessage)
    {
        _categoryName = categoryName;
        _integrationLogMessage = integrationLogMessage;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        //throw new NotImplementedException();
        return default!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        _integrationLogMessage?.Invoke(logLevel, _categoryName, eventId, formatter(state, exception), exception);
    }
}