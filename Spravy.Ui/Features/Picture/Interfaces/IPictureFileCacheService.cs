namespace Spravy.Ui.Features.Picture.Interfaces;

public interface IPictureFileCacheService
{
    Cvtar SaveFileAsync(FileInfo file, CancellationToken ct);
}