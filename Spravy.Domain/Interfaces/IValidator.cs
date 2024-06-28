namespace Spravy.Domain.Interfaces;

public interface IValidator<in TValue>
{
    IAsyncEnumerable<Result> ValidateAsync(TValue value, string sourceName);
}
