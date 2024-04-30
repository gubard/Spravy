namespace Spravy.Domain.Exceptions;

public class ParameterLengthToBigException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("54EA6C8B-F07E-4EDB-B213-A41468CF215D");

    public ParameterLengthToBigException(string parameterName, ushort maxLength, int length) : base(Identity,
        $"Parameter {parameterName} min length {maxLength}. Current length {length}")
    {
        ParameterName = parameterName;
        MaxLength = maxLength;
        Length = length;
    }

    public string ParameterName { get; }
    public ushort MaxLength { get; }
    public int Length { get; }
}