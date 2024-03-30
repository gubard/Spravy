using Spravy.Domain.Models;

namespace Spravy.Domain.ValidationResults;

public class UserWithLoginExistsValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("A5D95424-DEFF-4571-A740-E34E42B34DD8");
    
    public UserWithLoginExistsValidationResult() : base(MainId, "UserWithLoginExists")
    {
    }
}