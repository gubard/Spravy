namespace Spravy.ToDo.Domain.Models;

public readonly struct ReferenceToDoItemSettings
{
    public ReferenceToDoItemSettings(Guid referenceId)
    {
        ReferenceId = referenceId;
    }
    
    public Guid ReferenceId { get; }
}