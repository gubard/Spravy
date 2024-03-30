using Spravy.Domain.Models;

namespace Spravy.Domain.ValidationResults;

public class NotNullValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("C2DD60E2-E182-418A-891B-7A5C2140FAAC");

    public NotNullValidationResult() : base(MainId, "NotNull")
    {
    }
}