namespace Client;

public class GreetRepository: IGreetRepository
{
    private readonly ILogger<GreetRepository> _logger;

    public GreetRepository(ILogger<GreetRepository> logger)
    {
        _logger = logger;
    }

    public void SaveGreeting(string message)
    {
        _logger.LogInformation("Reply: {Message}", message);
    }
}


public interface IGreetRepository
{
    void SaveGreeting(string message);
}