using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace Spravy.Ui.Features.Picture.Models;

public class LocalToDoImage : IToDoImage, IDisposable
{
    private LocalToDoImage(Bitmap data)
    {
        Data = data;
    }

    public Bitmap Data { get; }

    public void Dispose()
    {
        Data.Dispose();
    }

    public static async ValueTask<Result<LocalToDoImage>> CreateAsync(IStorageFile file)
    {
        await using var imageStream = await file.OpenReadAsync();
        var data = new Bitmap(imageStream);

        return new LocalToDoImage(data).ToResult();
    }
}