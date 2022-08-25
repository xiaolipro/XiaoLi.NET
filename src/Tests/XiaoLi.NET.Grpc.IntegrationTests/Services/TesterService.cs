using Grpc.Core;
using Test;

namespace XiaoLi.NET.Grpc.IntegrationTests.Services;

public class TesterService:Tester.TesterBase
{
    private readonly IGreeterService _greeterService;

    public TesterService(IGreeterService greeterService)
    {
        _greeterService = greeterService;
    }
    
    public override Task<HelloReply> SayHelloUnary(HelloRequest request,
        ServerCallContext context)
    {
        var message = _greeterService.Greet(request.Name);
        return Task.FromResult(new HelloReply { Message = message });
    }

    public override async Task SayHelloServerStreaming(HelloRequest request,
        IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        var i = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var message = _greeterService.Greet($"{request.Name} {++i}");
            await responseStream.WriteAsync(new HelloReply { Message = message });

            await Task.Delay(1000);
        }
    }

    public override async Task<HelloReply> SayHelloClientStreaming(
        IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        var names = new List<string>();

        await foreach (var request in requestStream.ReadAllAsync())
        {
            names.Add(request.Name);
        }

        var message = _greeterService.Greet(string.Join(", ", names));
        return new HelloReply { Message = message };
    }

    public override async Task SayHelloBidirectionalStreaming(
        IAsyncStreamReader<HelloRequest> requestStream,
        IServerStreamWriter<HelloReply> responseStream,
        ServerCallContext context)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            await responseStream.WriteAsync(
                new HelloReply { Message = _greeterService.Greet(request.Name) });
        }
    }
}