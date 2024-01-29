using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IValidationRule<in TValue>
{
    Task<bool> ValidateAsync(TValue value, [MaybeNullWhen(true)] out ValidationResult result);
}