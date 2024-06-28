using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ReferenceToDoItemSettings
{
    public ReferenceToDoItemSettings(OptionStruct<Guid> referenceId)
    {
        ReferenceId = referenceId;
    }

    public OptionStruct<Guid> ReferenceId { get; }
}
