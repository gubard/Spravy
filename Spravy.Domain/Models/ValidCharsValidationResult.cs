namespace Spravy.Domain.Models;

public class ValidCharsValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("33A64668-4D6B-4A8C-B431-3ABD36C48E5B");
    
    public ValidCharsValidationResult(ReadOnlyMemory<char> validChars) : base(MainId)
    {
        ValidChars = validChars;
    }

    public ReadOnlyMemory<char> ValidChars { get; }
}