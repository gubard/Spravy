namespace Spravy.Domain.Models;

public class UnknownValidationResult : ValidationResult
{
    public static readonly Guid MainId = new("D02032B7-8B2A-4164-A347-7C2441E6D96D");
    
    public UnknownValidationResult(Guid validationResultId) : base(MainId, "Unknown")
    {
        ValidationResultId = validationResultId;
    }

    public Guid ValidationResultId { get; protected set; }
}