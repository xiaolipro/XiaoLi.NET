#if NETCOREAPP
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace XiaoLi.NET.Mvc.UnifiedResults
{
    public class BaseResult
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public int? Code { get; set; }
        
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }
        
        public BaseResult(int code = (int)HttpStatusCode.OK)
        {
            Code = code;
        }
    }
    
    public class DataResult<T> : BaseResult where T : class
    {
        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }
    }

    public class ErrorResult : BaseResult
    {
        public ErrorResult()
        {
            Code = (int)HttpStatusCode.InternalServerError;
        }

        public string StackTrace { get; set; }
    }
}
#endif