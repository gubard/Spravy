namespace Spravy.Picture.Domain.Models;

public readonly struct PictureResponse
{
    public PictureResponse(ReadOnlyMemory<PictureItem> pictures, ReadOnlyMemory<PictureData> data)
    {
        Pictures = pictures;
        Data = data;
    }

    public readonly ReadOnlyMemory<PictureItem> Pictures;
    public readonly ReadOnlyMemory<PictureData> Data;
}