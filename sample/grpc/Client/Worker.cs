using Test;

namespace Client;

public class Worker : BackgroundService
{
    private readonly Tester.TesterClient _client;
    private readonly IGreetRepository _greetRepository;

    public Worker(Tester.TesterClient client, IGreetRepository greetRepository)
    {
        _client = client;
        _greetRepository = greetRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            count++;

            var reply = await _client.SayHelloUnaryAsync(new HelloRequest { Name = $"Worker {count}" }, cancellationToken: stoppingToken);

            _greetRepository.SaveGreeting(reply.Message);

            await Task.Delay(1000, stoppingToken);
        }
    }
}