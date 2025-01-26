namespace Spravy.Picture.Domain.Models;

public readonly struct AddPicture : IDisposable, IAsyncDisposable
{
    public AddPicture(string name, string description, Stream data)
    {
        Name = name;
        Data = data;
        Description = description;
    }

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