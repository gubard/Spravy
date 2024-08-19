using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.EventBus.Domain.Mapper.Mappers;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Service.Services;

[Authorize]
public class GrpcEventBusService(EventStorage eventStorage, ILogger<GrpcEventBusService> logger)
    : EventBusService.EventBusServiceBase
{
    public override async Task<PublishEventReply> PublishEvent(
        PublishEventRequest request,
        ServerCallContext context
    )
    {
        var userId = context.GetHttpContext().GetUserId();
        var id = request.EventId.ToGuid();
        logger.LogInformation("{UserId} push event {Id}", userId, id);

        await eventStorage.AddEventAsync(
            id,
            request.Content.ToByteArray(),
            context.CancellationToken
        );

        return new();
    }

    public override async Task<GetEventsReply> GetEvents(
        GetEventsRequest request,
        ServerCallContext context
    )
    {
        var userId = context.GetHttpContext().GetUserId();
        var eventIds = request.EventIds.ToGuid();
        logger.LogInformation("{UserId} get events {EventIds}", userId, eventIds);
        var eventValues = await eventStorage.PushEventAsync(eventIds);
        var reply = new GetEventsReply();
        reply.Events.AddRange(eventValues.ToEvent().ToArray());

        return reply;
    }
}
