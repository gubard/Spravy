using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Domain.Interfaces;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Mapper.Mappers;
using Spravy.Picture.Protos;
using Spravy.Service.Extensions;

namespace Spravy.Picture.Service.Services;

[Authorize]
public class GrpcPictureService : Protos.PictureService.PictureServiceBase
{
    private readonly IPictureService pictureService;
    private readonly ISerializer serializer;

    public GrpcPictureService(IPictureService pictureService, ISerializer serializer)
    {
        this.pictureService = pictureService;
        this.serializer = serializer;
    }

    public override Task<EditPictureReply> EditPicture(EditPictureRequest request, ServerCallContext context)
    {
        return pictureService.EditPictureAsync(request.ToEditPicture(), context.CancellationToken)
           .HandleAsync<EditPictureReply>(serializer, context.CancellationToken);
    }

    public override Task<GetPictureReply> GetPicture(GetPictureRequest request, ServerCallContext context)
    {
        return pictureService.GetPictureAsync(request.ToGetPicture(), context.CancellationToken)
           .HandleAsync(serializer, response => response.ToGetPictureReply(), context.CancellationToken);
    }
}