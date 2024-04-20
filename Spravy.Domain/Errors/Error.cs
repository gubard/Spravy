namespace Spravy.Domain.Errors;

public abstract class Error
{
    protected Error(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
}