using Spravy.Domain.ValidationResults;

namespace Spravy.Domain.Interfaces;

public interface IValidator<in TValue>
{
    IAsyncEnumerable<Error> ValidateAsync(TValue value);
}