using Spravy.Domain.Models;

namespace Spravy.Picture.Domain.Models;

public readonly struct PictureParameters
{
    public PictureParameters(ReadOnlyMemory<Guid> ids, SizeDouble2D size)
    {
        Ids = ids;
        Size = size;
    }

    public ReadOnlyMemory<Guid> Ids { get; }
    public SizeDouble2D Size { get; } 
}