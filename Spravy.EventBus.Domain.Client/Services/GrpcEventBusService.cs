namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService : GrpcServiceBase<EventBusService.EventBusServiceClient>,
    IEventBusService,
    IGrpcServiceCreatorAuth<GrpcEventBusService, EventBusService.EventBusServiceClient>
{
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusService.EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    ) : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public Cvtar PublishEventAsync(Guid eventId, ReadOnlyMemory<byte> content, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new PublishEventRequest
                        {
                            EventId = eventId.ToByteString(),
                            Content = content.ToByteString(),
                        };

                        return client.PublishEventAsync(request, metadata, cancellationToken: ct)
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
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new GetEventsRequest();
                        request.EventIds.AddRange(eventIds.ToByteString().ToArray());

                        return client.GetEventsAsync(request, metadata, cancellationToken: ct)
                           .ToValueTaskResultValueOnly()
                           .ConfigureAwait(false)
                           .IfSuccessAsync(events => events.Events.ToEventValue().ToResult(), ct);
                    },
                    ct
                ),
            ct
        );
    }

    public static GrpcEventBusService CreateGrpcService(
        IFactory<Uri, EventBusService.EventBusServiceClient> grpcClientFactory,
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
}