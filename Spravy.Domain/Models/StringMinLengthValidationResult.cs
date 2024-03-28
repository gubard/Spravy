namespace Spravy.Domain.Models;

public class StringMinLengthValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("899C375F-FA4E-4A94-8034-62FCA6E91D93");

    public StringMinLengthValidationResult(ushort minLength) : base(MainId, "StringMinLength")
    {
        MinLength = minLength;
    }

    public ushort MinLength { get; }
}