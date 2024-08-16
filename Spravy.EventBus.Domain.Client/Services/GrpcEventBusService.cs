using System.Runtime.CompilerServices;
using Spravy.Client.Extensions;
using Spravy.Client.Helpers;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Interfaces;
using Spravy.Core.Mappers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Mapper.Mappers;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService
    : GrpcServiceBase<EventBusServiceClient>,
        IEventBusService,
        IGrpcServiceCreatorAuth<GrpcEventBusService, EventBusServiceClient>
{
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
        : base(grpcClientFactory, host, handler)
    {
        this.metadataFactory = metadataFactory;
    }

    public ConfiguredValueTaskAwaitable<Result> PublishEventAsync(
        Guid eventId,
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new PublishEventRequest
                            {
                                EventId = eventId.ToByteString(),
                                Content = content.ToByteString(),
                            };

                            return client
                                .PublishEventAsync(request, metadata, cancellationToken: ct)
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new GetEventsRequest();
                            request.EventIds.AddRange(eventIds.ToByteString().ToArray());

                            return client
                                .GetEventsAsync(request, metadata, cancellationToken: ct)
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    events => events.Events.ToEventValue().ToResult(),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public static GrpcEventBusService CreateGrpcService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler);
    }
}
