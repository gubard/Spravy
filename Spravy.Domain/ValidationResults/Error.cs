using System.ComponentModel;

namespace Spravy.Domain.ValidationResults;

public abstract class Error
{
    protected Error(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    [Category(nameof(Error)), DisplayName(nameof(Name))]
    public Guid Id { get; protected set; }

    [Category(nameof(Error)), DisplayName(nameof(Id))]
    public string Name { get; protected set; }
}