namespace Spravy.Picture.Domain.Models;

public readonly struct EditPicture
{
    public EditPicture(ReadOnlyMemory<AddPictureItem> addPictures)
    {
        AddPictures = addPictures;
    }

    public readonly ReadOnlyMemory<AddPictureItem> AddPictures;
}