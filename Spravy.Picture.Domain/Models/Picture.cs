using System.Runtime.CompilerServices;

namespace Spravy.Picture.Domain.Models;

public readonly struct Picture
{
    private Picture(Guid id, string name, string description, Stream data)
    {
        Id = id;
        Name = name;
        Data = data;
        Description = description;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Stream Data { get; }

    public static ConfiguredValueTaskAwaitable<Picture> CreateAsync(Guid id, string name, string description, Stream data)
    {
        return CreateCore(id, name, description, data).ConfigureAwait(false);
    }

    private static async ValueTask<Picture> CreateCore(Guid id, string name, string description, Stream data)
    {
        var stream = new MemoryStream();
        await data.CopyToAsync(stream);

        return new(id, name, description, data);
    }
}