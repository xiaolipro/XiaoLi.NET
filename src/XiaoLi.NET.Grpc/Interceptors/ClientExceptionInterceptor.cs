using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.Interceptors
{
    /// <summary>
    /// Grpc客户端异常拦截器
    /// </summary>
    public class ClientExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ClientExceptionInterceptor> _logger;

        public ClientExceptionInterceptor(ILogger<ClientExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var call = continuation(request, context);

            return new AsyncUnaryCall<TResponse>(HandleResponse(call.ResponseAsync), call.ResponseHeadersAsync, 
                call.GetStatus, call.GetTrailers, call.Dispose);
        }

        private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> responseAsync)
        {
            try
            {
                var response = await responseAsync;
                return response;
            }
            catch (RpcException e)
            {
                _logger.LogError("通过Grpc调用时发生异常: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
        }
    }
}
