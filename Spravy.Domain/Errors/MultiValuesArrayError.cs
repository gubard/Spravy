namespace Spravy.Domain.Errors;

public class MultiValuesArrayError : Error
{
    public static readonly Guid MainId = new("702461E9-A88C-4E9E-BD65-C07C976EF9CC");

    protected MultiValuesArrayError()
        : base(MainId)
    {
        Name = string.Empty;
    }

    public MultiValuesArrayError(string name, ulong arrayLength)
        : base(MainId)
    {
        Name = name;
        ArrayLength = arrayLength;
    }

    public string Name { get; protected set; }
    public ulong ArrayLength { get; protected set; }

    public override string Message
    {
        get => $"Array {Name} has {ArrayLength} values";
    }
}
