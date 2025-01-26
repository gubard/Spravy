using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Enums;
using Spravy.Picture.Domain.Interfaces;

namespace Spravy.Picture.Domain.Services;

public class PictureEditor : IPictureEditor
{

    public Cvtar ResizeImageAsync(
        Stream stream,
        double size,
        SizeType type,
        FileInfo saveFile,
        CancellationToken ct
    )
    {
        return ResizeImageCore(
                stream,
                size,
                type,
                saveFile,
                ct
            )
           .ConfigureAwait(false);
    }

    private async ValueTask<Result> ResizeImageCore(
        Stream stream,
        double size,
        SizeType type,
        FileInfo saveFile,
        CancellationToken ct
    )
    {
        using var image = await Image.LoadAsync(stream, ct);
        var scale = GetScale(image.Size, size, type);
        image.Mutate(x => x.Resize(image.Width * scale, image.Height * scale));
        await image.SaveAsWebpAsync(saveFile.FullName, ct);

        return Result.Success;
    }

    private int GetScale(
        Size imageSize,
        double size,
        SizeType type
    )
    {
        return type switch
        {
            SizeType.Width => (int)Math.Round(imageSize.Width / size),
            SizeType.Height => (int)Math.Round(imageSize.Height / size),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}