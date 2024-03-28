using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService : GrpcServiceBase<EventBusServiceClient>,
    IEventBusService,
    IGrpcServiceCreatorAuth<GrpcEventBusService, EventBusServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            (client, token) => SubscribeEventsAsyncCore(client, eventIds, token),
            cancellationToken
        );
    }

    public Task PublishEventAsync(Guid eventId, byte[] content, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new PublishEventRequest
                {
                    EventId = mapper.Map<ByteString>(eventId),
                    Content = ByteString.CopyFrom(content),
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.PublishEventAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<EventValue>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetEventsRequest();
                request.EventIds.AddRange(mapper.Map<IEnumerable<ByteString>>(eventIds.ToArray()));
                cancellationToken.ThrowIfCancellationRequested();
                var events = await client.GetEventsAsync(request, metadata, cancellationToken: cancellationToken);

                return mapper.Map<IEnumerable<EventValue>>(events.Events);
            },
            cancellationToken
        );
    }

    private async IAsyncEnumerable<EventValue> SubscribeEventsAsyncCore(
        EventBusServiceClient client,
        Guid[] eventIds,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var request = new SubscribeEventsRequest();
        request.EventIds.AddRange(mapper.Map<IEnumerable<ByteString>>(eventIds));
        cancellationToken.ThrowIfCancellationRequested();
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.SubscribeEvents(request, metadata, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var eventValue = mapper.Map<EventValue>(reply);

            yield return eventValue;

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public static GrpcEventBusService CreateGrpcService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
}