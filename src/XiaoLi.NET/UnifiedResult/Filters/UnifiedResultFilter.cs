#if NETCOREAPP3_0_OR_GREATER
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XiaoLi.NET.UnifiedResult.Attributes;

namespace XiaoLi.NET.UnifiedResult.Filters
{
    /// <summary>
    /// 统一结果过滤器（只处理成功请求）
    /// </summary>
    public class UnifiedResultFilter:IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ed = await next();
            
            if (ed.Exception != null && !ed.ExceptionHandled) return;
            
            if (context.ActionDescriptor.EndpointMetadata.Any(
                    x => x.GetType() == typeof(SuppressUnifiedResultAttribute))) return;
            
            if (ValidResult(ed.Result, out object data))
            {
                ed.Result = new ObjectResult(new UnifiedResponse()
                {
                    Code = StatusCode.Success,
                    Message = "成功",
                    Data = data
                });
            }
        }

        private bool ValidResult(IActionResult actionResult, out object o)
        {
            if (actionResult is JsonResult jr)
            {
                o = jr.Value;
                return jr.Value is not UnifiedResponse;
            }
            if (actionResult is ObjectResult or)
            {
                o = or.Value;
                return or.Value is not UnifiedResponse;
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