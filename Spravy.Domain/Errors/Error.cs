namespace Spravy.Domain.Errors;

public abstract class Error
{
    protected Error(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; protected set; }
    public abstract string Message { get; }
}