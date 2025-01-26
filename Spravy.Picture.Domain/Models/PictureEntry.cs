namespace Spravy.Picture.Domain.Models;

public readonly struct PictureEntry : IDisposable, IAsyncDisposable
{
    public PictureEntry(Guid id, Stream data)
    {
        Id = id;
        Data = data;
    }

    public readonly Guid Id;
    public readonly Stream Data;

    public void Dispose()
    {
        Data.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Data.DisposeAsync();
    }
}