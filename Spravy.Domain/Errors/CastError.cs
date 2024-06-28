namespace Spravy.Domain.Errors;

public class CastError : Error
{
    public static readonly Guid MainId = new("2856F946-9D57-4830-8DD7-D45C434F3D11");

    protected CastError()
        : base(MainId)
    {
        InputType = typeof(UnknownType);
        OutputType = typeof(UnknownType);
    }

    public CastError(Type inputType, Type outputType)
        : base(MainId)
    {
        InputType = inputType;
        OutputType = outputType;
    }

    public override string Message
    {
        get => $"Can't cast {InputType} to {OutputType}.";
    }

    public Type InputType { get; protected set; }
    public Type OutputType { get; protected set; }
}
