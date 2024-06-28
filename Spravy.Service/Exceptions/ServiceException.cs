namespace Spravy.Service.Exceptions;

public abstract class ServiceException : SpravyException
{
    protected ServiceException(Guid id, string massage, Metadata metadata)
        : base(id, massage)
    {
        Metadata = metadata;
    }

    public Metadata Metadata { get; }
}
