using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService : GrpcServiceBase, IEventBusService
{
    private readonly EventBusServiceClient client;
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        GrpcEventBusServiceOptions options,
        IMapper mapper,
        IMetadataFactory metadataFactory
    ) : base(
        options.Host.ThrowIfNullOrWhiteSpace().ToUri(),
        options.ChannelType,
        options.ChannelCredentialType.GetChannelCredentials()
    )
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
        client = new EventBusServiceClient(GrpcChannel);
    }

    public async IAsyncEnumerable<EventValue> SubscribeEventsAsync(
        Guid[] eventIds,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var request = new SubscribeEventsRequest();
        request.EventIds.AddRange(mapper.Map<IEnumerable<ByteString>>(eventIds));
        using var response = client.SubscribeEvents(request, await metadataFactory.CreateAsync());
        cancellationToken.ThrowIfCancellationRequested();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var eventValue = mapper.Map<EventValue>(reply);

            yield return eventValue;

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task PublishEventAsync(Guid eventId, byte[] content)
    {
        try
        {
            var request = new PublishEventRequest
            {
                EventId = mapper.Map<ByteString>(eventId),
                Content = ByteString.CopyFrom(content),
            };

            await client.PublishEventAsync(request, await metadataFactory.CreateAsync());
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }
}