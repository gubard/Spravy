using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.ValidationResults;

namespace Spravy.Domain.Interfaces;

public interface IValidationRule<in TValue>
{
    Task<bool> ValidateAsync(TValue value, [MaybeNullWhen(true)] out Error result);
}