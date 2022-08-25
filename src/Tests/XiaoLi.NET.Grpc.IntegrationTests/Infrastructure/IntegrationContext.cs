using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace XiaoLi.NET.Grpc.IntegrationTests.Infrastructure;

public class IntegrationContext<TStartup> : IDisposable where TStartup : class
{
    private readonly IntegrationFixture<TStartup> _fixture;
    private readonly ITestOutputHelper _outputHelper;
    private readonly Stopwatch _stopwatch;

    public IntegrationContext(IntegrationFixture<TStartup> fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _outputHelper = outputHelper;
        _stopwatch = Stopwatch.StartNew();
        _fixture.LogMessage += WriteMessage;
    }

    private void WriteMessage(LogLevel level, string category, EventId id, string message, Exception exception) =>
        _outputHelper.WriteLine($"{_stopwatch.Elapsed.TotalSeconds:N4}s {category} - {level}: {message}");

    public void Dispose()
    {
        //_fixture.Dispose();
        _fixture.LogMessage -= WriteMessage;
    }
}