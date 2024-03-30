using Spravy.Domain.Models;
using Spravy.Domain.ValidationResults;

namespace Spravy.Domain.Interfaces;

public interface IValidator<in TValue>
{
    IAsyncEnumerable<ValidationResult> ValidateAsync(TValue value);
}