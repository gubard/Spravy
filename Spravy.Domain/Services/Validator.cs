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
        for (var index = 0; index < rules.Span.Length; index++)
        {
            var result = await rules.Span[index].ValidateAsync(value, sourceName);

            if (result.IsHasError)
            {
                yield return result;
            }
        }
    }
}