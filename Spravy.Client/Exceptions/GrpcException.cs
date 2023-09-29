namespace Spravy.Client.Exceptions;

public class GrpcException : Exception
{
    public GrpcException(Uri host, Exception? innerException) : base($"{host} throw exception.", innerException)
    {
        Host = host;
    }

    public Uri Host { get; }
}