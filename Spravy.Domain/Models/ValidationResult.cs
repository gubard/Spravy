namespace Spravy.Domain.Models;

public abstract class ValidationResult
{
    protected ValidationResult(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}