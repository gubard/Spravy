using System.Diagnostics.CodeAnalysis;
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

    public Task<bool> ValidateAsync(string? value, [MaybeNullWhen(true)] out ValidationResult result)
    {
        if (value is null)
        {
            result = null;

            return true.ToTaskResult();
        }

        if (value.Length < minLength)
        {
            result = new StringMinLengthValidationResult(minLength);

            return false.ToTaskResult();
        }

        result = null;

        return true.ToTaskResult();
    }
}