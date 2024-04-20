using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class StringMaxLengthValidationRule : IValidationRule<string>
{
    private readonly ushort maxLength;

    public StringMaxLengthValidationRule(ushort maxLength)
    {
        this.maxLength = maxLength;
    }

    public ConfiguredValueTaskAwaitable<Result> ValidateAsync(string? value, string sourceName)
    {
        if (value is null)
        {
            return Result.AwaitableFalse;
        }

        if (value.Length > maxLength)
        {
            return new Result(new StringMaxLengthError(maxLength)).ToValueTaskResult().ConfigureAwait(false);
        }

        return Result.AwaitableFalse;
    }
}