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
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            (client, token) => SubscribeEventsAsyncCore(client, eventIds, token).ConfigureAwait(false),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> PublishEventAsync(
        Guid eventId,
        ReadOnlyMemory<byte> content,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(
                metadata =>
                {
                    var request = new PublishEventRequest
                    {
                        EventId = eventId.ToByteString(),
                        Content = content.ToByteString(),
                    };

                    return client.PublishEventAsync(request, metadata, cancellationToken: cancellationToken)
                       .ToValueTaskResultOnly()
                       .ConfigureAwait(false);
                }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata =>
            {
                var request = new GetEventsRequest();
                request.EventIds.AddRange(eventIds.ToByteString().ToArray());

                return client.GetEventsAsync(request, metadata, cancellationToken: cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(
                        events => events.Events.ToEventValue().ToResult(), cancellationToken);
            }, cancellationToken), cancellationToken);
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
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var request = new SubscribeEventsRequest();
        var eventIdsByteString = eventIds.ToByteString();
        request.EventIds.AddRange(eventIdsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.SubscribeEvents(request, metadata.Value, cancellationToken: cancellationToken);

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var eventValue = reply.ToEventValue();

            yield return eventValue.ToResult();
        }
    }
}