using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IValidator<in TValue>
{
    IAsyncEnumerable<ValidationResult> ValidateAsync(TValue value);
}