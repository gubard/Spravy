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

public class GrpcEventBusService : GrpcServiceBase<EventBusServiceClient>, IEventBusService, IGrpcServiceCreator<GrpcEventBusService, EventBusServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    ) : base(grpcClientFactory, host)
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

    public Task PublishEventAsync(Guid eventId, byte[] content)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new PublishEventRequest
                {
                    EventId = mapper.Map<ByteString>(eventId),
                    Content = ByteString.CopyFrom(content),
                };

                await client.PublishEventAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task<IEnumerable<EventValue>> GetEventsAsync(ReadOnlyMemory<Guid> eventIds)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetEventsRequest();
                request.EventIds.AddRange(mapper.Map<IEnumerable<ByteString>>(eventIds.ToArray()));
                var events = await client.GetEventsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<EventValue>>(events.Events);
            }
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
        var metadata = await metadataFactory.CreateAsync();
        using var response = client.SubscribeEvents(request, metadata);
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
        IMetadataFactory metadataFactory
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory);
    }
}