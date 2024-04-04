namespace Spravy.Domain.Errors;

public class NotNullError : Error
{
    public static readonly Guid MainId = new("C2DD60E2-E182-418A-891B-7A5C2140FAAC");

    public NotNullError() : base(MainId, "NotNull")
    {
    }
}