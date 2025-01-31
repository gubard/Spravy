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

        if (scale < 1)
        {
            image.Mutate(x => x.Resize((int)(image.Width * scale), (int)(image.Height * scale)));
        }

        await image.SaveAsWebpAsync(saveFile.FullName, ct);

        return Result.Success;
    }

    private double GetScale(
        Size imageSize,
        double size,
        SizeType type
    )
    {
        return type switch
        {
            SizeType.Width => size / imageSize.Width,
            SizeType.Height => size / imageSize.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}