namespace Spravy.Domain.Exceptions;

public class ParameterWhiteSpaceException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("F4101610-FF61-44B8-BFB5-83C10BF83E23");

    public ParameterWhiteSpaceException(string parameterName)
        : base(Identity, "Parameter can't be white space")
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}
