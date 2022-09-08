using Grpc.Core;
using Server;

namespace Server.Services;

public class GreeterService : IGreeterService
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public string Greet(string name)
    {
        if (name.Contains("9")) throw new Exception("GG");
        _logger.LogInformation("Creating greeting to {Name}", name);
        return $"Hello {name}";
    }
}

public interface IGreeterService
{
    string Greet(string name);
}