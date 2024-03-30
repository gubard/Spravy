namespace Spravy.Domain.ValidationResults;

public class StringMinLengthError : Error
{
    public static readonly Guid MainId = new("899C375F-FA4E-4A94-8034-62FCA6E91D93");

    public StringMinLengthError(ushort minLength) : base(MainId, "StringMinLength")
    {
        MinLength = minLength;
    }

    public ushort MinLength { get; protected set; }
}