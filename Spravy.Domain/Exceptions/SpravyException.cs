namespace Spravy.Domain.Exceptions;

public abstract class SpravyException : Exception
{
    protected SpravyException(Guid id, string massage) : base(massage)
    {
        Id = id;
    }

    public Guid Id { get; }
}