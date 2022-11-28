#if NETCOREAPP
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using XiaoLi.NET.Mvc.Attributes;

namespace XiaoLi.NET.Mvc.Filters
{
    /// <summary>
    /// 统一包装action-result
    /// </summary>
    public class UnifiedResultFilter : IAsyncResultFilter
    {
        private readonly IUnifiedResultHandler _unifiedResultHandler;

        public UnifiedResultFilter(IUnifiedResultHandler unifiedResultHandler)
        {
            _unifiedResultHandler = unifiedResultHandler;
        }

        public virtual async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(
                    x => x.GetType() == typeof(SuppressUnifiedResultAttribute))) return;

            _unifiedResultHandler.HandleActionResult(context);

            await next();
        }
    }
}
#endif