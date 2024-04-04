using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class StringMaxLengthValidationRule : IValidationRule<string>
{
    private readonly ushort maxLength;

    public StringMaxLengthValidationRule(ushort maxLength)
    {
        this.maxLength = maxLength;
    }

    public Task<bool> ValidateAsync(string? value, [MaybeNullWhen(true)] out Error result)
    {
        if (value is null)
        {
            result = null;

            return true.ToTaskResult();
        }

        if (value.Length > maxLength)
        {
            result = new StringMaxLengthError(maxLength);

            return false.ToTaskResult();
        }

        result = null;

        return true.ToTaskResult();
    }
}