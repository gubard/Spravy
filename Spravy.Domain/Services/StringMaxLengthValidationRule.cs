using System.Diagnostics.CodeAnalysis;
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

    public Task<bool> ValidateAsync(string? value, [MaybeNullWhen(true)] out ValidationResult result)
    {
        if (value is null)
        {
            result = null;

            return true.ToTaskResult();
        }

        if (value.Length > maxLength)
        {
            result = new StringMaxLengthValidationResult(maxLength);

            return false.ToTaskResult();
        }

        result = null;

        return true.ToTaskResult();
    }
}