namespace Spravy.Picture.Domain.Models;

public readonly struct PictureResponse
{
    public PictureResponse(ReadOnlyMemory<PictureItem> pictures)
    {
        Pictures = pictures;
    }

    public readonly ReadOnlyMemory<PictureItem> Pictures;
}