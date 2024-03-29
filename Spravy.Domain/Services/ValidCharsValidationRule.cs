using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class ValidCharsValidationRule : IValidationRule<string>
{
    private readonly ReadOnlyMemory<char> _validChars;

    public ValidCharsValidationRule(ReadOnlyMemory<char> validChars)
    {
        _validChars = validChars;
    }

    public Task<bool> ValidateAsync(string? value, [MaybeNullWhen(true)] out ValidationResult result)
    {
        if (value is null)
        {
            result = null;

            return true.ToTaskResult();
        }

        if (value.AsSpan().IndexOfAnyExcept(_validChars.Span) == -1)
        {
            result = null;
            return true.ToTaskResult();
        }

        result = new ValidCharsValidationResult(_validChars);

        return false.ToTaskResult();
    }
}