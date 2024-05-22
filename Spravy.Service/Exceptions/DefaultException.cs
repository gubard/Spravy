namespace Spravy.Service.Exceptions;

public class DefaultException : ServiceException
{
    public DefaultException(Metadata metadata, ServiceException exception) : base(exception.Id, exception.Message,
        metadata)
    {
        Exception = exception;
    }

    public ServiceException Exception { get; }
}