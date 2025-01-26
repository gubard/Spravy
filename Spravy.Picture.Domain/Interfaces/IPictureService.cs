using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Models;

namespace Spravy.Picture.Domain.Interfaces;

public interface IPictureService
{
    ConfiguredValueTaskAwaitable<Result<PictureResponse>> GetPictureAsync(GetPicture getPicture, CancellationToken ct);
    Cvtar EditPictureAsync(EditPicture editPicture, CancellationToken ct);
}