using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class NotNullValidationRule<TObject> : IValidationRule<TObject> where TObject : class
{
    public Task<bool> ValidateAsync(TObject? value, [MaybeNullWhen(true)] out ValidationResult result)
    {
        if (value is null)
        {
            result = DefaultObject<NotNullValidationResult>.Default;

            return false.ToTaskResult();
        }

        result = null;

        return true.ToTaskResult();
    }
}