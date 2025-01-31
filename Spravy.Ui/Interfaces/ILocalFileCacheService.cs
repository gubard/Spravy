using Spravy.Picture.Domain.Enums;

namespace Spravy.Ui.Interfaces;

public interface ILocalFileCacheService
{
    ConfiguredValueTaskAwaitable<Result<Stream>> GetPictureStreamAsync(Guid pictureId, SizeType type, ushort size);
    ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(Guid pictureId, SizeType type, ushort size);
    Cvtar SavePictureStreamAsync(Guid pictureId, SizeType type, ushort size, Stream stream);
}