namespace Spravy.Domain.Models;

public class StringMaxLengthValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("B554DD15-82E1-4B54-AEB4-88CFF95CCCEA");

    public StringMaxLengthValidationResult(ushort maxLength) : base(MainId)
    {
        MaxLength = maxLength;
    }

    public ushort MaxLength { get; }
}