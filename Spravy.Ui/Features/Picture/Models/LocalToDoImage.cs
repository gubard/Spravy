using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Spravy.Picture.Domain.Enums;

namespace Spravy.Ui.Features.Picture.Models;

public class LocalToDoImage : IToDoImage
{
    private LocalToDoImage(Bitmap data, Stream rawData, string name)
    {
        Data = data;
        RawData = rawData;
        Name = name;
    }

    public Bitmap Data { get; }
    public Stream RawData { get; }
    public string Name { get; }

    public void Dispose()
    {
        Data.Dispose();
        RawData.Dispose();
    }

    public static async ValueTask<Result<LocalToDoImage>> CreateAsync(IStorageFile file, SizeType type, ushort size, CancellationToken ct)
    {
        await using var imageStream = await file.OpenReadAsync();
        var stream = new MemoryStream();
        await imageStream.CopyToAsync(stream, ct);
        stream.Seek(0, SeekOrigin.Begin);

        var data = type switch
        {
            SizeType.Width => Bitmap.DecodeToWidth(stream, size),
            SizeType.Height => Bitmap.DecodeToHeight(stream, size),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };

        stream.Seek(0, SeekOrigin.Begin);

        return new LocalToDoImage(data, stream, Path.GetFileNameWithoutExtension(file.Name)).ToResult();
    }
}