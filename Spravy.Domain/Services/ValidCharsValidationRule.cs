namespace Spravy.Domain.Services;

public class ValidCharsValidationRule : IValidationRule<string>
{
    private readonly ReadOnlyMemory<char> validChars;

    public ValidCharsValidationRule(ReadOnlyMemory<char> validChars)
    {
        this.validChars = validChars;
    }

    public Cvtar ValidateAsync(string? value, string sourceName)
    {
        if (value is null)
        {
            return Result.AwaitableSuccess;
        }

        if (value.AsSpan().IndexOfAnyExcept(validChars.Span) == -1)
        {
            return Result.AwaitableSuccess;
        }

        return new Result(new VariableInvalidCharsError(validChars, sourceName)).ToValueTaskResult()
           .ConfigureAwait(false);
    }
}