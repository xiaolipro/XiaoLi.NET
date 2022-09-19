using System;

namespace XiaoLi.NET.Mvc.Exceptions
{
    public class BussinessException:Exception
    {
        public BussinessException ()
        { }

        public BussinessException (string message)
            : base(message)
        { }

        public BussinessException (string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
