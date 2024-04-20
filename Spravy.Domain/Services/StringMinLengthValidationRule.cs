using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class StringMinLengthValidationRule : IValidationRule<string>
{
    private readonly ushort minLength;

    public StringMinLengthValidationRule(ushort minLength)
    {
        this.minLength = minLength;
    }

    public ConfiguredValueTaskAwaitable<Result> ValidateAsync(string? value, string sourceName)
    {
        if (value is null)
        {
            return Result.AwaitableFalse;
        }

        if (value.Length < minLength)
        {
            return new Result(new StringMinLengthError(minLength)).ToValueTaskResult().ConfigureAwait(false);
        }

        return Result.AwaitableFalse;
    }
}