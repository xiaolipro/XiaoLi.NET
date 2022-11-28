#if NETCOREAPP
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XiaoLi.NET.Mvc.Exceptions;
using XiaoLi.NET.Mvc.UnifiedResults;

namespace XiaoLi.NET.Mvc.Filters
{
    public class HttpExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<HttpExceptionFilter> _logger;

        public HttpExceptionFilter(IHostEnvironment env,ILogger<HttpExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }
        
        public virtual Task OnExceptionAsync(ExceptionContext context)
        {
            HandleExceptionResult(context);
            return Task.CompletedTask;
        }
        
        private void HandleExceptionResult(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            
            if (context.ExceptionHandled) return;
            
            if (context.Exception is BusinessException businessException)
            {
                var res = UnifiedResultFactory.CreateBaseResult();
                res.Code = (int)HttpStatusCode.BadRequest;
                res.Message = businessException.Message;

                context.Result = new BadRequestObjectResult(res);
            }
            else
            {
                var res = UnifiedResultFactory.CreateErrorResult();
                res.Message = context.Exception.Message;
                if (_env.IsDevelopment()) res.StackTrace = context.Exception.StackTrace;

                // Result assigned to a result object but in destiny the response is empty. This is a known bug of .net core 1.1
                // It will be fixed in .net core 1.1.2. See https://github.com/aspnet/Mvc/issues/5594 for more information
                context.Result = new ObjectResult(res);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;
        }
    }
}
#endif