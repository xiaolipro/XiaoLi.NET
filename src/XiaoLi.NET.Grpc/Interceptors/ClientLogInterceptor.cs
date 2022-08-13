using Grpc.Core.Interceptors;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc.Interceptors
{
    /// <summary>
    /// Grpc客户端日志拦截器
    /// </summary>
    /// <remarks>
    /// <para>Description see: https://docs.microsoft.com/zh-cn/aspnet/core/grpc/interceptors?view=aspnetcore-6.0</para>
    /// <para>Impl see: https://github.com/grpc/grpc-dotnet/blob/master/examples/Interceptor/Server/ServerLoggerInterceptor.cs</para>
    /// </remarks>
    public class ClientLogInterceptor : Interceptor
    {
        private readonly ILogger<ClientLogInterceptor> _logger;

        public ClientLogInterceptor(ILogger<ClientLogInterceptor> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// 截获一元 RPC 的阻塞调用。
        /// </summary>
        /// <remarks>
        /// Tips: 尽管 BlockingUnaryCall 和 AsyncUnaryCall 都是指一元 RPC，但二者不可互换。 
        /// 阻塞调用不会被 AsyncUnaryCall 截获，异步调用不会被 BlockingUnaryCall 截获。
        /// </remarks>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(
            TRequest request, 
            ClientInterceptorContext<TRequest, TResponse> context, 
            BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            WriteLog(context);
            AddCallerMetadata(ref context);

            // => return continuation(request, context);
            return base.BlockingUnaryCall(request, context, continuation);
        }


       

        /// <summary>
        /// 截获一元 RPC 的异步调用。
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request, 
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            WriteLog(context);
            AddCallerMetadata(ref context);

            var call = continuation(request, context);

            return new AsyncUnaryCall<TResponse>(
                HandleResponse(call.ResponseAsync),
                call.ResponseHeadersAsync,
                call.GetStatus,
                call.GetTrailers,
                call.Dispose);
        }

        /// <summary>
        /// 截获客户端流式处理 RPC 的异步调用。
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
            ClientInterceptorContext<TRequest, TResponse> context, 
            AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            WriteLog(context);
            AddCallerMetadata(ref context);

            return base.AsyncClientStreamingCall(context, continuation);
        }

        /// <summary>
        /// 截获服务器流式处理 RPC 的异步调用。
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            WriteLog(context);
            AddCallerMetadata(ref context);

            return base.AsyncServerStreamingCall(request, context, continuation);
        }

        /// <summary>
        /// 截获双向流式处理 RPC 的异步调用。
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            WriteLog(context);
            AddCallerMetadata(ref context);
            return base.AsyncDuplexStreamingCall(context, continuation);
        }


        /// <summary>
        /// 添加调用者元数据到请求头
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context"></param>
        private void AddCallerMetadata<TRequest, TResponse>(ref ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            var headers = context.Options.Headers;


            // 如果当前上下文没有headers，创建带有headers的新上下文
            if (headers is null)
            {
                headers = new Metadata();
                var options = context.Options.WithHeaders(headers);

                context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);
            }

            // 添加调用者metadata到请求头
            headers.Add("caller-user", Environment.UserName);
            headers.Add("caller-machine", Environment.MachineName);
            headers.Add("caller-os", Environment.OSVersion.ToString());
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="context"></param>
        private void WriteLog<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class
            where TResponse : class
        {
            _logger.LogInformation($"开始调用，主机{context.Host}，类型：{context.Method.Type}，方法：{context.Method.Name}，" +
                         $"请求：{typeof(TRequest)}，响应：{typeof(TResponse)}");
        }




        /// <summary>
        /// 处理异步调用
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="responseAsync"></param>
        /// <returns></returns>
        private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> responseAsync)
            where TResponse : class
        {
            try
            {
                var response = await responseAsync;

                _logger.LogInformation($"接收响应：{response}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"调用发生异常：{ex.Message}\n {ex.StackTrace}");
                throw;
            }
        }
    }
}
