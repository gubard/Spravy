namespace Spravy.Domain.Errors;

public class EmptyArrayError : Error
{
    public static readonly Guid MainId = new("36636FA3-48DE-4C2E-951D-953C8669259D");

    protected EmptyArrayError() : base(MainId)
    {
        Name = string.Empty;
    }

    public EmptyArrayError(string name) : base(MainId)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    public override string Message
    {
        get => $"Array {Name} is empty";
    }
}