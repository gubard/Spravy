using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using Spravy.EventBus.Service.Interfaces;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Service.Services;

public class GrpcEventBusService : EventBusServiceBase
{
    private readonly IEventPusher eventPusher;
    private readonly IMapper mapper;

    public GrpcEventBusService(IEventPusher eventPusher, IMapper mapper)
    {
        this.eventPusher = eventPusher;
        this.mapper = mapper;
    }

    public override async Task SubscribeEvents(
        SubscribeEventsRequest request,
        IServerStreamWriter<SubscribeEventsReply> responseStream,
        ServerCallContext context
    )
    {
        var eventIds = mapper.Map<Guid[]>(request.EventIds);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var eventValue = await WaitAnyEventAsync(eventIds, context.CancellationToken);

            if (!eventValue.IsSuccess)
            {
                return;
            }

            var reply = new SubscribeEventsReply
            {
                EventId = mapper.Map<ByteString>(eventValue.Value.Id),
                Content = await ByteString.FromStreamAsync(eventValue.Value.Stream),
            };

            await responseStream.WriteAsync(reply);
        }
    }

    public override async Task<PublishEventReply> PublishEvent(PublishEventRequest request, ServerCallContext context)
    {
         await using var stream = new MemoryStream(request.Content.ToByteArray());
        await eventPusher.PublishEventAsync(mapper.Map<Guid>(request.EventId), stream);

        return new PublishEventReply();
    }

    private async ValueTask<IsSuccessValue<EventValue>> WaitAnyEventAsync(
        Guid[] eventIds,
        CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var eventValue = await eventPusher.WaitEventAsync();

            if (eventIds.Contains(eventValue.Id))
            {
                return new IsSuccessValue<EventValue>(eventValue);
            }
        }

        return new IsSuccessValue<EventValue>();
    }
}