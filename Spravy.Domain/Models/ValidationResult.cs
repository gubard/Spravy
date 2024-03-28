using System.ComponentModel;

namespace Spravy.Domain.Models;

public abstract class ValidationResult
{
    protected ValidationResult(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    [Category(nameof(ValidationResult)), DisplayName(nameof(Name))]
    public Guid Id { get; protected set; }

    [Category(nameof(ValidationResult)), DisplayName(nameof(Id))]
    public string Name { get; protected set; }
}