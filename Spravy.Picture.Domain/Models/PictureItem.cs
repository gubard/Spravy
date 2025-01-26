namespace Spravy.Picture.Domain.Models;

public readonly struct PictureItem
{
    public PictureItem(string entry, Guid id, Picture picture)
    {
        Entry = entry;
        Id = id;
        Picture = picture;
    }

    public readonly string Entry;
    public readonly Guid Id;
    public readonly Picture Picture;
}