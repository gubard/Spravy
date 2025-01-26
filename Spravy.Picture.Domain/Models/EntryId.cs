namespace Spravy.Picture.Domain.Models;

public readonly struct EntryId
{
    public EntryId(string entry, ReadOnlyMemory<Guid> ids)
    {
        Ids = ids;
        Entry = entry;
    }

    public readonly ReadOnlyMemory<Guid> Ids;
    public readonly string Entry;
}