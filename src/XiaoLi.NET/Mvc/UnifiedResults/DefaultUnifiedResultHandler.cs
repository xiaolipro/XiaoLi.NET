#if NETCOREAPP
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XiaoLi.NET.Mvc.Filters;

namespace XiaoLi.NET.Mvc.UnifiedResults
{
    public class DefaultUnifiedResultHandler:IUnifiedResultHandler
    {
        public void HandleActionResult(ResultExecutingContext context)
        {
            // void、Task会被包装成EmptyResult
            if (context.Result is EmptyResult)
            {
                var res = ResultFactory.CreateSimpleResult();
                context.Result = new ObjectResult(res);

                return;
            }
            
            // string、int、list以及自定义的模型，都会被包装成为ObjectResult
            if (context.Result is ObjectResult objectResult)
            {
                var value = objectResult.Value;
                
                // 已unified的无须再包装
                if (value is SimpleResult) return;

                var res = ResultFactory.CreateDataResult<object>();

                // A machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.
                if (value is ProblemDetails details)
                {
                    res.Code = details.Status;
                    res.Message = details.Title;
                    res.Data = context.ModelState.Keys
                        .SelectMany(key => context.ModelState[key].Errors
                            .Where(x => !string.IsNullOrWhiteSpace(key))
                            .Select(x => new
                            {
                                Field = key,
                                Message = x.ErrorMessage
                            }))
                        .ToList();
                    objectResult.Value = res;
                    return;
                }

                res.Data = value;
                objectResult.Value = res;
                
                // 解决string格式问题
                objectResult.DeclaredType = res.GetType();
            }
        }
    }
}
#endif