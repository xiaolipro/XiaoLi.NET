﻿using System.Threading.Channels;
using Grpc.Core;

namespace ServerUnitTests;

public class CustomServerStreamWriter<T> : IServerStreamWriter<T> where T : class
{
    private readonly ServerCallContext _serverCallContext;
    private readonly Channel<T> _channel;
    
    public WriteOptions? WriteOptions { get; set; }
    
    public CustomServerStreamWriter(ServerCallContext serverCallContext)
    {
        _channel = Channel.CreateUnbounded<T>();

        _serverCallContext = serverCallContext;
    }
    
    
    public async Task<T?> ReadNextAsync()
    {
        if (await _channel.Reader.WaitToReadAsync())
        {
            _channel.Reader.TryRead(out var message);
            return message;
        }

        return null;
    }
    public IAsyncEnumerable<T> ReadAllAsync()
    {
        return _channel.Reader.ReadAllAsync();
    }
    
    public Task WriteAsync(T message)
    {
        if (_serverCallContext.CancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(_serverCallContext.CancellationToken);
        }

        if (!_channel.Writer.TryWrite(message))
        {
            throw new InvalidOperationException("Unable to write message.");
        }

        return Task.CompletedTask;
    }
    
    public void Complete()
    {
        _channel.Writer.Complete();
    }
}