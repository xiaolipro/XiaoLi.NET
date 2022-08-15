using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.Interceptors;

/// <summary>
/// Grpc服务端异常拦截器
/// </summary>
public class ServerExceptionInterceptor:Interceptor
{
    private readonly ILogger<ServerExceptionInterceptor> _logger;

    public ServerExceptionInterceptor(ILogger<ServerExceptionInterceptor> logger)
    {
        _logger = logger;
    }
        
    /// <summary>
    /// 截获一元 RPC。
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <param name="continuation"></param>
    /// <returns></returns>
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, 
        ServerCallContext context, 
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await base.UnaryServerHandler(request, context, continuation);
        }
        catch (RpcException e)
        {
            _logger.LogError("通过Grpc调用时发生异常: {Status} - {Message}", e.Status, e.Message);
            return default;
        }
            
    }
}