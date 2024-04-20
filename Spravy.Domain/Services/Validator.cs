using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public abstract class Validator<TValue> : IValidator<TValue>
{
    private readonly ReadOnlyMemory<IValidationRule<TValue>> rules;

    public Validator(ReadOnlyMemory<IValidationRule<TValue>> rules)
    {
        this.rules = rules;
    }

    public async IAsyncEnumerable<Result> ValidateAsync(TValue value, string sourceName)
    {
        foreach (var rule in rules.Span.ToArray())
        {
            var result = await rule.ValidateAsync(value, sourceName);

            if (result.IsHasError)
            {
                yield return result;
            }
        }
    }
}