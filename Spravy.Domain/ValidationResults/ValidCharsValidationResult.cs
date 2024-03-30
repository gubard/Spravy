using Spravy.Domain.Models;

namespace Spravy.Domain.ValidationResults;

public class ValidCharsValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("33A64668-4D6B-4A8C-B431-3ABD36C48E5B");
    
    public ValidCharsValidationResult(ReadOnlyMemory<char> validChars) : base(MainId, "ValidChars")
    {
        ValidChars = validChars;
    }

    public ReadOnlyMemory<char> ValidChars { get; protected set; }
}