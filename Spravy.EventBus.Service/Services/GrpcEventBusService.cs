using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.EventBus.Domain.Mapper.Mappers;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Service.Services;

[Authorize]
public class GrpcEventBusService : EventBusService.EventBusServiceBase
{
    private readonly EventStorage eventStorage;
    private readonly ISerializer serializer;

    public GrpcEventBusService(EventStorage eventStorage, ISerializer serializer)
    {
        this.eventStorage = eventStorage;
        this.serializer = serializer;
    }

    public override Task<PublishEventReply> PublishEvent(
        PublishEventRequest request,
        ServerCallContext context
    )
    {
        return eventStorage
            .AddEventAsync(
                request.EventId.ToGuid(),
                request.Content.ToByteArray(),
                context.CancellationToken
            )
            .HandleAsync<PublishEventReply>(serializer, context.CancellationToken);
    }

    public override Task<GetEventsReply> GetEvents(
        GetEventsRequest request,
        ServerCallContext context
    )
    {
        return eventStorage
            .PushEventAsync(request.EventIds.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                events =>
                {
                    var reply = new GetEventsReply();
                    reply.Events.AddRange(events.ToEvent().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }
}
