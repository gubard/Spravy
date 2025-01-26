namespace Spravy.Picture.Domain.Models;

public readonly struct Picture : IDisposable, IAsyncDisposable
{
    public Picture(Guid id, string name, string description, Stream data)
    {
        Id = id;
        Name = name;
        Data = data;
        Description = description;
    }

    public readonly Guid Id;
    public readonly string Name;
    public readonly string Description;
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