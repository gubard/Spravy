namespace Spravy.Domain.Errors;

public class NotImplementedError : Error
{
    public static readonly Guid MainId = new("9209E412-CAD9-4C8A-B808-B3DB2DA526E1");

    protected NotImplementedError()
        : base(MainId)
    {
        Name = string.Empty;
    }

    public NotImplementedError(string name)
        : base(MainId)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    public override string Message
    {
        get => $"Function not implemented \"{Name}\"";
    }
}
