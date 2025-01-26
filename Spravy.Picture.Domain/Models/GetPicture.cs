namespace Spravy.Picture.Domain.Models;

public readonly struct GetPicture
{
    public GetPicture(ReadOnlyMemory<IdPictureParameters> pictures)
    {
        Pictures = pictures;
    }

    public readonly ReadOnlyMemory<IdPictureParameters> Pictures;
}