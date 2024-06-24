using System.Runtime.CompilerServices;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
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

public class GrpcEventBusService : GrpcServiceBase<EventBusServiceClient>,
    IEventBusService,
    IGrpcServiceCreatorAuth<GrpcEventBusService, EventBusServiceClient>
{
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.metadataFactory = metadataFactory;
    }

    public ConfiguredCancelableAsyncEnumerable<Result<EventValue>> SubscribeEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            (client, token) => SubscribeEventsAsyncCore(client, eventIds, token).ConfigureAwait(false),
            ct);
    }

    public ConfiguredValueTaskAwaitable<Result> PublishEventAsync(
        Guid eventId,
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(ct)
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
                }, ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken ct
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(ct)
           .IfSuccessAsync(metadata =>
            {
                var request = new GetEventsRequest();
                request.EventIds.AddRange(eventIds.ToByteString().ToArray());

                return client.GetEventsAsync(request, metadata, cancellationToken: ct)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(
                        events => events.Events.ToEventValue().ToResult(), ct);
            }, ct), ct);
    }

    public static GrpcEventBusService CreateGrpcService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, metadataFactory, serializer);
    }

    private async IAsyncEnumerable<Result<EventValue>> SubscribeEventsAsyncCore(
        EventBusServiceClient client,
        ReadOnlyMemory<Guid> eventIds,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        var request = new SubscribeEventsRequest();
        var eventIdsByteString = eventIds.ToByteString();
        request.EventIds.AddRange(eventIdsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(ct);
        using var response = client.SubscribeEvents(request, metadata.Value, cancellationToken: ct);

        while (await response.ResponseStream.MoveNext(ct))
        {
            var reply = response.ResponseStream.Current;
            var eventValue = reply.ToEventValue();

            yield return eventValue.ToResult();
        }
    }
}