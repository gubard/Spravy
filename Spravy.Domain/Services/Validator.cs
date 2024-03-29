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

    public async IAsyncEnumerable<ValidationResult> ValidateAsync(TValue value)
    {
        foreach (var rule in rules.Span.ToArray())
        {
            if (!await rule.ValidateAsync(value, out var result))
            {
                yield return result;
            }
        }
    }
}