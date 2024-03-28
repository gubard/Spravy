namespace Spravy.Domain.Models;

public class UserWithEmailExistsValidationResult : ValidationResult
{
    public static readonly UserWithEmailExistsValidationResult Default = new();
    public static readonly Guid MainId = new("2759AC7A-45D9-4D3F-9762-C145568C5F00");

    public UserWithEmailExistsValidationResult() : base(MainId, "UserWithEmailExists")
    {
    }
}