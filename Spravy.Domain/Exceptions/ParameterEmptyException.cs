namespace Spravy.Domain.Exceptions;

public class ParameterEmptyException: SpravyException
{
    public static readonly Guid Identity = Guid.Parse("0BA8150F-39EA-4845-844A-42BC3793D5B6");

    public ParameterEmptyException( string parameterName) : base(Identity, $"Parameter {parameterName} can't be empty")
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}