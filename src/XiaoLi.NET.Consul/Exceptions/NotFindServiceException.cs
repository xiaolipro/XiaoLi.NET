using System.Runtime.Serialization;
using System;

namespace XiaoLi.NET.App.Consul.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NotFindServiceException : Exception
    {
        public NotFindServiceException()
        {
        }

        public NotFindServiceException(string message) : base(message)
        {
        }

        public NotFindServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotFindServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}