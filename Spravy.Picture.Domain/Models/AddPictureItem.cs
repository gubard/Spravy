namespace Spravy.Picture.Domain.Models;

public readonly struct AddPictureItem
{
    public AddPictureItem(EntryId entryId, ReadOnlyMemory<AddPicture> items)
    {
        EntryId = entryId;
        Items = items;
    }

    public readonly EntryId EntryId;
    public readonly ReadOnlyMemory<AddPicture> Items;
}