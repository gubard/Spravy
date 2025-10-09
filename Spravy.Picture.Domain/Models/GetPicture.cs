namespace Spravy.Picture.Domain.Models;

public readonly struct GetPicture
{
    public GetPicture(ReadOnlyMemory<EntryId> entryIds, ReadOnlyMemory<PictureParameter> parameters)
    {
        EntryIds = entryIds;
        Parameters = parameters;
    }

    public readonly ReadOnlyMemory<EntryId> EntryIds;
    public readonly ReadOnlyMemory<PictureParameter> Parameters;
}