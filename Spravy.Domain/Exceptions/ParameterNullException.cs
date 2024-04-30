namespace Spravy.Domain.Exceptions;

public class ParameterNullException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("75B9D72D-4264-4A95-8F1E-91F1CDE5110D");

    public ParameterNullException(string parameterName) : base(Identity, $"Parameter {parameterName} can't be null")
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}