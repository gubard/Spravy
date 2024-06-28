namespace Spravy.Domain.Exceptions;

public class ParameterLengthToSmallException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("E0D1E558-C2C8-44A2-B49B-DB2D9DCFD9B7");

    public ParameterLengthToSmallException(string parameterName, ushort minLength, int length)
        : base(
            Identity,
            $"Parameter {parameterName} min length {minLength}. Current length {length}"
        )
    {
        ParameterName = parameterName;
        MinLength = minLength;
        Length = length;
    }

    public string ParameterName { get; }
    public ushort MinLength { get; }
    public int Length { get; }
}
