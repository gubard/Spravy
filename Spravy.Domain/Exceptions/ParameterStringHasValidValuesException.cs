namespace Spravy.Domain.Exceptions;

public class ParameterStringHasValidValuesException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("215C1CD5-AD65-43BD-918F-5F01316993D4");

    public ParameterStringHasValidValuesException(
        string parameterName,
        char[] validValues,
        char invalid
    )
        : base(
            Identity,
            $"Parameter {parameterName} has invaild char {invalid}. {parameterName} can contains {new(validValues)}"
        )
    {
        ParameterName = parameterName;
        ValidValues = validValues;
        Invalid = invalid;
    }

    public string ParameterName { get; }
    public char Invalid { get; }
    public char[] ValidValues { get; }
}
