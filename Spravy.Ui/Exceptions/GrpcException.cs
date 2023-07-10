using System;

namespace Spravy.Ui.Exceptions;

public class GrpcException : Exception
{
    public GrpcException(string target, Exception? innerException) : base($"{target} throw exception.", innerException)
    {
        Target = target;
    }

    public string Target { get; }
}