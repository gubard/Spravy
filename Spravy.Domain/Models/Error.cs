namespace Spravy.Domain.Models;

public readonly struct Error
{
    public Error(ReadOnlyMemory<ValidationResult> validationResults)
    {
        ValidationResults = validationResults;
    }

    public ReadOnlyMemory<ValidationResult> ValidationResults { get; }

    public bool IsError => !ValidationResults.IsEmpty;
}