using Spravy.Picture.Domain.Enums;

namespace Spravy.Picture.Domain.Models;

public readonly struct PictureParameter
{
    public PictureParameter(Guid id, string entry, SizeType type, ushort size)
    {
        Id = id;
        Type = type;
        Size = size;
        Entry = entry;
    }

    public readonly Guid Id;
    public readonly string Entry;
    public readonly SizeType Type;
    public readonly ushort Size;
}