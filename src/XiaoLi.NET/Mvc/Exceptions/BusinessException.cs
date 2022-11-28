#if NETCOREAPP
using System;

namespace XiaoLi.NET.Mvc.Exceptions
{
    public class BusinessException:Exception
    {
        public BusinessException ()
        { }

        public BusinessException (string message)
            : base(message)
        { }

        public BusinessException (string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

#endif