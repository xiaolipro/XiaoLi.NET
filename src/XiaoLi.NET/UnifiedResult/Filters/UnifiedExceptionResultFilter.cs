#if NETCOREAPP
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XiaoLi.NET.UnifiedResult.Filters
{
    public class UnifiedExceptionResultFilter : IAsyncExceptionFilter
    {
        private readonly IUnifiedResultFactory _unifiedResultFactory;

        public UnifiedExceptionResultFilter(IUnifiedResultFactory unifiedResultFactory)
        {
            _unifiedResultFactory = unifiedResultFactory;
        }
        
        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                
            }
            
            
            
            return Task.CompletedTask;
        }
    }


    public class BussinessException
    {
        
    }
}
#endif