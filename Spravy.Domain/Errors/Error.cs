namespace Spravy.Domain.Errors;

public abstract class Error
{
    protected Error(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
    public abstract string Message { get; }

    public override string ToString()
    {
        return Message;
    }
}
