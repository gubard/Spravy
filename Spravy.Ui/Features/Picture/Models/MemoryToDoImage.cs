using Avalonia.Media.Imaging;

namespace Spravy.Ui.Features.Picture.Models;

public class MemoryToDoImage : IToDoImage
{
    public MemoryToDoImage(Guid id, ReadOnlyMemory<byte> data)
    {
        Id = id;
        using var stream = data.ToMemoryStream();
        Data = new(stream);
    }

    public Guid Id { get; }
    public Bitmap Data { get; }

    public void Dispose()
    {
        Data.Dispose();
    }
}