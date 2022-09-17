#if NETCOREAPP
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XiaoLi.NET.UnifiedResult.Attributes;
using XiaoLi.NET.UnifiedResult.Factories;

namespace XiaoLi.NET.UnifiedResult.Filters
{
    /// <summary>
    /// 统一正常结果过滤器
    /// </summary>
    public class UnifiedSuccessResultFilter:IAsyncActionFilter
    {
        private readonly IUnifiedResultFactory _unifiedResultFactory;

        public UnifiedSuccessResultFilter(IUnifiedResultFactory unifiedResultFactory)
        {
            _unifiedResultFactory = unifiedResultFactory;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ed = await next();
            
            // 未处理的异常交由异常过滤器捕获
            if (ed.Exception != null && !ed.ExceptionHandled) return;
            
            if (context.ActionDescriptor.EndpointMetadata.Any(
                    x => x.GetType() == typeof(SuppressUnifiedResultAttribute))) return;
            
            if (ValidResult(ed.Result, out object data))
            {
                ed.Result = _unifiedResultFactory.GenerateSuccessResult(data);
            }
        }

        private bool ValidResult(IActionResult actionResult, out object o)
        {
            if (actionResult is JsonResult jr)
            {
                o = jr.Value;
                return true;
            }
            if (actionResult is ObjectResult or)
            {
                o = or.Value;
                return true;
            }
            if (actionResult is EmptyResult)
            {
                o = null;
                return true;
            }
            
            o = null;
            return false;
        }
    }
}
#endif