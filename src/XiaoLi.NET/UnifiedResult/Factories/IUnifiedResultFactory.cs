#if NETCOREAPP
using System;
using Microsoft.AspNetCore.Mvc;

namespace XiaoLi.NET.UnifiedResult.Factories
{
    public interface IUnifiedResultFactory
    {
        SimpleResult CreateSimpleResult();

        ExceptionResult CreateExceptionResult();
    }
}
#endif