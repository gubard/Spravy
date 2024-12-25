using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Domain.Interfaces;

public interface IPictureService
{
    ConfiguredValueTaskAwaitable<Result<Guid>> AddPicturesAsync(ReadOnlyMemory<AddPicture> picture);
    Cvtar DeletePicturesAsync(ReadOnlyMemory<Guid> picture);

    ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<Models.Picture>>> GetPicturesAsync(
        ReadOnlyMemory<PictureParameters> parameters
    );
}