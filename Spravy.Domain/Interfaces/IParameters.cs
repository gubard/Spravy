namespace Spravy.Domain.Interfaces;

public interface IParameters
{
    Result<string> GetParameter(ReadOnlySpan<char> parameterName);
}