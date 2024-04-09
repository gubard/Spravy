namespace Spravy.Domain.Errors;

public class CanceledByUserError : Error
{
    public static readonly Guid MainId = new("51DE4267-F4B4-4B50-B711-24319CCA7B5A");

    public CanceledByUserError() : base(MainId, "Canceled by user")
    {
    }
}