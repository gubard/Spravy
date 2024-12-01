namespace Spravy.Domain.Errors;

public class NotFoundNamedError : Error
{
    public static readonly Guid MainId = new("7DEBE73E-2128-4F1B-A331-AACA4ED9F8D9");

    protected NotFoundNamedError() : base(MainId)
    {
        Name = string.Empty;
    }

    public NotFoundNamedError(string name) : base(MainId)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    public override string Message => $"Not found by name \"{Name}\"";
}