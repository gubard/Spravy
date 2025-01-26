using Avalonia.Media.Imaging;

namespace Spravy.Ui.Features.Picture.Models;

public class MemoryToDoImage : IToDoImage, IDisposable
{
    public MemoryToDoImage(Stream stream)
    {
        Data = new(stream);
    }

    public Bitmap Data { get; }

    public void Dispose()
    {
        Data.Dispose();
    }
}