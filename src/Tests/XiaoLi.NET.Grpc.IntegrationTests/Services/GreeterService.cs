using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.IntegrationTests.Services;

public class GreeterService : IGreeterService
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public string Greet(string name)
    {
        _logger.LogInformation("欢迎您，{Name}", name);
        return $"Hello {name}";
    }
}

public interface IGreeterService
{
    string Greet(string name);
}