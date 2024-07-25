namespace Spravy.Domain.Interfaces;

public interface IObjectParameters
{
    Result<string> GetParameter(ReadOnlySpan<char> parameterName);
    Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue);
}
