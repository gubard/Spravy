namespace Spravy.Picture.Domain.Models;

public readonly struct PictureData
{
    public PictureData(Guid id, ReadOnlyMemory<byte> data)
    {
        Id = id;
        Data = data;
    }

    public readonly Guid Id;
    public readonly ReadOnlyMemory<byte> Data;
}