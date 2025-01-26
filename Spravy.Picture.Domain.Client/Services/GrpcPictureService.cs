using System.Runtime.CompilerServices;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Picture.Domain.Interfaces;
using Spravy.Picture.Domain.Mapper.Mappers;
using Spravy.Picture.Domain.Models;
using Spravy.Picture.Protos;

namespace Spravy.Picture.Domain.Client.Services;

public class GrpcPictureService : GrpcServiceBase<PictureService.PictureServiceClient>,
    IPictureService,
    IGrpcServiceCreatorAuth<GrpcPictureService, PictureService.PictureServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
    private readonly IMetadataFactory metadataFactory;

    public GrpcPictureService(
        IFactory<Uri, PictureService.PictureServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    ) : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcPictureService CreateGrpcService(
        IFactory<Uri, PictureService.PictureServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(
            grpcClientFactory,
            host,
            metadataFactory,
            handler,
            retryService
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PictureResponse>> GetPictureAsync(
        GetPicture getPicture,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.GetPictureAsync(
                            getPicture.ToGetPictureRequest(),
                            metadata,
                            DateTime.UtcNow.Add(Timeout),
                            ct
                        )
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(
                            reply => reply.ToPictureResponse().ToResult(),
                            ct
                        ),
                    ct
                ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> EditPictureAsync(EditPicture editPicture, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.EditPictureAsync(
                            editPicture.ToEditPictureRequest(),
                            metadata,
                            DateTime.UtcNow.Add(Timeout),
                            ct
                        )
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .ToResultOnlyAsync(),
                    ct
                ),
            ct
        );
    }
}