namespace Spravy.Domain.Errors;

public class StringMaxLengthError : Error
{
    public static readonly Guid MainId = new("B554DD15-82E1-4B54-AEB4-88CFF95CCCEA");

    public StringMaxLengthError(ushort maxLength) : base(MainId, "StringMaxLength")
    {
        MaxLength = maxLength;
    }

    public ushort MaxLength { get; protected set; }
}