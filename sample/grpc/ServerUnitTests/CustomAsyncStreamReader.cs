using System.Threading.Channels;
using Grpc.Core;
using Channel = System.Threading.Channels.Channel;

namespace ServerUnitTests;

public class CustomAsyncStreamReader<T> : IAsyncStreamReader<T> where T : class
{
    private readonly Channel<T> _channel;
    private readonly ServerCallContext _serverCallContext;

    public T Current { get; private set; } = null!;
    
    public CustomAsyncStreamReader(ServerCallContext serverCallContext)
    {
        _channel = Channel.CreateUnbounded<T>();
        _serverCallContext = serverCallContext;
    }
    
    public void AddMessage(T message)
    {
        if (!_channel.Writer.TryWrite(message))
        {
            throw new InvalidOperationException("Unable to write message.");
        }
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        _serverCallContext.CancellationToken.ThrowIfCancellationRequested();

        if (await _channel.Reader.WaitToReadAsync(cancellationToken) &&
            _channel.Reader.TryRead(out var message))
        {
            Current = message;
            return true;
        }

        Current = null!;
        return false;
    }

}