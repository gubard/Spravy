namespace Spravy.Domain.Models;

public abstract class ValidationResult
{
    protected ValidationResult(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
}