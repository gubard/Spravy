using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class ValidCharsValidationRule : IValidationRule<string>
{
    private readonly ReadOnlyMemory<char> validChars;

    public ValidCharsValidationRule(ReadOnlyMemory<char> validChars)
    {
        this.validChars = validChars;
    }

    public ConfiguredValueTaskAwaitable<Result> ValidateAsync(string? value, string sourceName)
    {
        if (value is null)
        {
            return Result.AwaitableFalse;
        }

        if (value.AsSpan().IndexOfAnyExcept(validChars.Span) == -1)
        {
            return Result.AwaitableFalse;
        }

        return new Result(new VariableInvalidCharsError(validChars, sourceName)).ToValueTaskResult()
           .ConfigureAwait(false);
    }
}