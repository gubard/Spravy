namespace Spravy.Domain.Errors;

public class CastError : Error
{
    public static readonly Guid MainId = new("2856F946-9D57-4830-8DD7-D45C434F3D11");

    public CastError(Type inputType, Type outputType) : base(MainId, $"Can't cast {inputType} to {outputType}.")
    {
        InputType = inputType;
        OutputType = outputType;
    }

    public Type InputType { get; protected set; }
    public Type OutputType { get; protected set; }
}