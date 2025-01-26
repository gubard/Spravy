using Spravy.Picture.Domain.Enums;

namespace Spravy.Picture.Domain.Interfaces;

public interface IPictureEditor
{
    Cvtar ResizeImageAsync(
        Stream stream,
        double size,
        SizeType type,
        FileInfo saveFile,
        CancellationToken ct
    );
}