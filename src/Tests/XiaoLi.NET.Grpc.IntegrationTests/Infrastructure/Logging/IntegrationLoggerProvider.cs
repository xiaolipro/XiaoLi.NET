using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.IntegrationTests.Infrastructure.Logging;

public class IntegrationLoggerProvider:ILoggerProvider
{
    private readonly IntegrationLogMessage? _integrationLogMessage;

    public IntegrationLoggerProvider(IntegrationLogMessage? integrationLogMessage)
    {
        _integrationLogMessage = integrationLogMessage;
    }
    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new IntegrationLogger(categoryName, _integrationLogMessage);
    }
}