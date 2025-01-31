using Spravy.Picture.Domain.Models;

namespace Spravy.Ui.Features.Picture.Interfaces;

public interface IPictureUiService
{
    event Func<PictureResponse, Cvtar>? Requested;
    ConfiguredValueTaskAwaitable<Result<PictureResponse>> GetRequest(GetPicture get, CancellationToken ct);
}