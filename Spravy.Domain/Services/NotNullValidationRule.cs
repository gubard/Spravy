using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class NotNullValidationRule<TObject> : IValidationRule<TObject> where TObject : class
{
    public Task<bool> ValidateAsync(TObject? value, [MaybeNullWhen(true)] out Error result)
    {
        if (value is null)
        {
            result = DefaultObject<NotNullError>.Default;

            return false.ToTaskResult();
        }

        result = null;

        return true.ToTaskResult();
    }
}