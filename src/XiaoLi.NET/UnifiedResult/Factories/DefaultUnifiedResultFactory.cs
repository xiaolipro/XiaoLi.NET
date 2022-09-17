#if NETCOREAPP
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace XiaoLi.NET.UnifiedResult.Factories
{
    public class DefaultUnifiedResultFactory : IUnifiedResultFactory
    {
        public IUnifiedResult GenerateSuccessResult(object data)
        {
            return new ObjectResult(new
            {
                Code = HttpStatusCode.OK,
                Message = "成功",
                Data = data
            });
        }

        public IUnifiedResult GenerateExceptionResult(Exception exception)
        {
            return new ObjectResult(new
            {
                Code = HttpStatusCode.InternalServerError,
                Message = exception.Message,
                MessageDetails = exception.StackTrace
            });
        }

        public IUnifiedResult CreateSimpleResult() => new SimpleResult();

        public IUnifiedResult CreateExceptionResult()
        {
            throw new NotImplementedException();
        }
    }
}
#endif