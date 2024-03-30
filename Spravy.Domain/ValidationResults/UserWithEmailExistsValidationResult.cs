using Spravy.Domain.Models;

namespace Spravy.Domain.ValidationResults;

public class UserWithEmailExistsValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("2759AC7A-45D9-4D3F-9762-C145568C5F00");

    public UserWithEmailExistsValidationResult() : base(MainId, "UserWithEmailExists")
    {
    }
}