using System.Collections.Concurrent;
using System.Threading.Channels;
using BenchmarkDotNet.Attributes;

namespace BenchmarkTests;

[MemoryDiagnoser]
public class ChannelBenchmark
{
    private readonly Channel<string> _channel;
    private readonly ConcurrentQueue<string> _queue;

    public ChannelBenchmark()
    {
        _channel = Channel.CreateUnbounded<string>();
        _queue = new ConcurrentQueue<string>();
    }

    [Benchmark]
    public async Task ChannelRWAsync()
    {
        await _channel.Writer.WriteAsync("hello");
        var _ = await _channel.Reader.ReadAsync();
    }
    
    [Benchmark]
    public void ChannelRW()
    {
        _channel.Writer.TryWrite("hello");
        _channel.Reader.TryRead(out _);
    }
    
    [Benchmark]
    public void ConcurrentQueueRW()
    {
        _queue.Enqueue("hello");
        _queue.TryDequeue(out _);
    }
}