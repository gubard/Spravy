using Spravy.Picture.Domain.Enums;

namespace Spravy.Picture.Domain.Models;

public readonly struct IdPictureParameters
{
    public IdPictureParameters(ReadOnlyMemory<EntryId> entryIds, double size, SizeType type)
    {
        Size = size;
        Type = type;
        EntryIds = entryIds;
    }

    public readonly double Size;
    public readonly SizeType Type;
    public readonly ReadOnlyMemory<EntryId> EntryIds;
}