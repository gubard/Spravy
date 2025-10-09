using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Models;
using Spravy.Ui.Features.Picture.Interfaces;

namespace Spravy.Ui.Features.Picture.Services;

public class PictureUiService : IPictureUiService
{
    private readonly IPictureService pictureService;
    private readonly IPictureFileCacheService pictureFileCacheService;

    public PictureUiService(IPictureService pictureService, IPictureFileCacheService pictureFileCacheService)
    {
        this.pictureService = pictureService;
        this.pictureFileCacheService = pictureFileCacheService;
    }

    public event Func<PictureResponse, Cvtar>? Requested;

    public ConfiguredValueTaskAwaitable<Result<PictureResponse>> GetRequest(GetPicture get, CancellationToken ct)
    {
        return pictureService.GetPictureAsync(get, ct)
           .IfSuccessAsync(
                response => Requested is null
                    ? response.ToResult().ToValueTaskResult().ConfigureAwait(false)
                    : Requested.Invoke(response).IfSuccessAsync(() => response.ToResult(), ct),
                ct
            );
    }

    private Result UpdatePictures(PictureResponse response, CancellationToken ct)
    {
        return Result.Success;
    }
}