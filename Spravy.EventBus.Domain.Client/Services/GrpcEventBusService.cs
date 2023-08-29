using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Authentication.Domain.Models;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService : GrpcServiceBase, IEventBusService
{
    private readonly EventBusServiceClient client;
    private readonly IKeeper<TokenResult> tokenKeeper;
    private readonly IMapper mapper;

    public GrpcEventBusService(
        GrpcEventBusServiceOptions options,
        IMapper mapper,
        IKeeper<TokenResult> tokenKeeper
    ) : base(options.Host.ToUri(), options.ChannelType, options.ChannelCredentialType.GetChannelCredentials())
    {
        this.mapper = mapper;
        this.tokenKeeper = tokenKeeper;
        client = new EventBusServiceClient(grpcChannel);
    }

    public async IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken)
    {
        var request = new SubscribeEventsRequest();
        request.EventIds.AddRange(mapper.Map<IEnumerable<ByteString>>(eventIds));
        using var response = client.SubscribeEvents(request, CreateMetadata());
        cancellationToken.ThrowIfCancellationRequested();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;

            yield return mapper.Map<EventValue>(reply);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task PublishEventAsync(Guid eventId, Stream content)
    {
        try
        {
            var request = new PublishEventRequest
            {
                EventId = mapper.Map<ByteString>(eventId),
                Content = await ByteString.FromStreamAsync(content),
            };

            await client.PublishEventAsync(request);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    private Metadata CreateMetadata()
    {
        var metadata = new Metadata
        {
            {
                "Authorization", $"Bearer {tokenKeeper.Get().Token}"
            }
        };

        return metadata;
    }
}