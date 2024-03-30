using Spravy.Domain.ValidationResults;

namespace Spravy.Domain.Models;

public readonly struct Result
{
    public Result(ReadOnlyMemory<ValidationResult> validationResults)
    {
        ValidationResults = validationResults;
    }

    public ReadOnlyMemory<ValidationResult> ValidationResults { get; }

    public bool IsHasError => !ValidationResults.IsEmpty;
}

public readonly struct Result<TValue>
{
    public Result(ReadOnlyMemory<ValidationResult> validationResults)
    {
        ValidationResults = validationResults;
    }

    public Result(TValue value)
    {
        Value = value;
    }

    public ReadOnlyMemory<ValidationResult> ValidationResults { get; }
    public TValue Value { get; }

    public bool IsHasError => !ValidationResults.IsEmpty;
}