using System.Runtime.CompilerServices;

namespace Spravy.Picture.Domain.Models;

public readonly struct AddPicture
{
    private AddPicture(string name, string description, Stream data)
    {
        Name = name;
        Data = data;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
    public Stream Data { get; }

    public static ConfiguredValueTaskAwaitable<AddPicture> CreateAsync(string name, string description, Stream data)
    {
        return CreateCore(name, description, data).ConfigureAwait(false);
    }

    private static async ValueTask<AddPicture> CreateCore(string name, string description, Stream data)
    {
        var stream = new MemoryStream();
        await data.CopyToAsync(stream);

        return new(name, description, data);
    }
}