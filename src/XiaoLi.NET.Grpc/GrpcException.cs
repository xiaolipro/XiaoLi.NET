using System;
using System.Runtime.Serialization;

namespace XiaoLi.NET.Grpc;

/// <summary>
/// Grpc发生异常
/// </summary>
public class GrpcException: Exception
{
    public GrpcException()
    {
    }

    public GrpcException(string message) : base(message)
    {
    }

    public GrpcException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected GrpcException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}