#if NETCOREAPP
using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace XiaoLi.NET.Mvc.UnifiedResults
{
    public static class UnifiedResultFactory
    {
        public static BaseResult CreateBaseResult()
        {
            return new BaseResult();
        }

        public static DataResult<T> CreateDataResult<T>() where T : class
        {
            return new DataResult<T>();
        }

        public static ErrorResult CreateErrorResult()
        {
            return new ErrorResult();
        }
    }
}
#endif