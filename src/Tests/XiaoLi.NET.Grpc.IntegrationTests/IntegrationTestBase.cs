using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.Grpc.IntegrationTests.Infrastructure;
using Xunit.Abstractions;

namespace XiaoLi.NET.Grpc.IntegrationTests;

public class IntegrationTestBase:IClassFixture<IntegrationFixture<Startup>>, IDisposable
{
    private readonly IDisposable _integrationContext;
    private GrpcChannel? _channel;
    
    protected IntegrationFixture<Startup> IntegrationFixture { get; }
    protected GrpcChannel? Channel => _channel ??= CreateChannel();

    private GrpcChannel? CreateChannel()
    {
        var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions()
        {
            LoggerFactory = IntegrationFixture.LoggerFactory,
            HttpHandler = IntegrationFixture.Handler
        });

        return channel;
    }

    public IntegrationTestBase(IntegrationFixture<Startup> integrationFixture, ITestOutputHelper outputHelper)
    {
        IntegrationFixture = integrationFixture;
        _integrationContext = new IntegrationContext<Startup>(integrationFixture, outputHelper);
    }


    public void Dispose()
    {
        _integrationContext.Dispose();
        _channel = null;
        //IntegrationFixture.Dispose();
    }
}