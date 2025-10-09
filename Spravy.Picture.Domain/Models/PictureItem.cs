namespace Spravy.Picture.Domain.Models;

public readonly struct PictureItem
{
    public PictureItem(string entry, Guid id, PictureInfo info)
    {
        Entry = entry;
        Id = id;
        Info = info;
    }

    public readonly string Entry;
    public readonly Guid Id;
    public readonly PictureInfo Info;
}