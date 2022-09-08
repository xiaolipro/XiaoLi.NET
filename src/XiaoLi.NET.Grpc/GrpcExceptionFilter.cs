using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace XiaoLi.NET.Grpc;

public class GrpcExceptionFilter:IExceptionFilter
{
    private readonly ILogger<GrpcExceptionFilter> _logger;

    public GrpcExceptionFilter(ILogger<GrpcExceptionFilter> logger)
    {
        _logger = logger;
    }
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        
        if (context.GetType() == typeof(GrpcException))
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }
}