using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Mapper.Mappers;
using Spravy.EventBus.Protos;
using Spravy.Service.Extensions;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcRouterEventBusService : EventBusService.EventBusServiceBase
{
    private readonly IEventBusService eventBusService;
    private readonly ISerializer serializer;
    private readonly ILogger<GrpcRouterEventBusService> logger;

    public GrpcRouterEventBusService(
        ILogger<GrpcRouterEventBusService> logger,
        IEventBusService eventBusService,
        ISerializer serializer
    )
    {
        this.logger = logger;
        this.eventBusService = eventBusService;
        this.serializer = serializer;
    }

    public override async Task SubscribeEvents(
        SubscribeEventsRequest request,
        IServerStreamWriter<SubscribeEventsReply> responseStream,
        ServerCallContext context
    )
    {
        var eventIds = request.EventIds.ToGuid();
        var userId = context.GetHttpContext().GetUserId();
        logger.LogInformation("{UserId} subscribe events {EventIds}", userId, eventIds);
        var events = eventBusService.SubscribeEventsAsync(eventIds, context.CancellationToken);

        await foreach (var @event in events)
        {
            var subscribeEventsReply = @event.ThrowIfError().ToSubscribeEventsReply();
            await responseStream.WriteAsync(subscribeEventsReply, context.CancellationToken);
        }
    }

    public override async Task<PublishEventReply> PublishEvent(
        PublishEventRequest request,
        ServerCallContext context
    )
    {
        var userId = context.GetHttpContext().GetUserId();
        var id = request.EventId.ToGuid();
        logger.LogInformation("{UserId} push event {Id}", userId, id);
        await eventBusService.PublishEventAsync(
            id,
            request.Content.ToByteArray(),
            context.CancellationToken
        );

        return new();
    }

    public override Task<GetEventsReply> GetEvents(
        GetEventsRequest request,
        ServerCallContext context
    )
    {
        var userId = context.GetHttpContext().GetUserId();
        var eventIds = request.EventIds.ToGuid();
        logger.LogInformation("{UserId} get events {EventIds}", userId, eventIds);

        return eventBusService
            .GetEventsAsync(eventIds, context.CancellationToken)
            .HandleAsync(
                serializer,
                eventValues =>
                {
                    var reply = new GetEventsReply();
                    reply.Events.AddRange(eventValues.ToEvent().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }
}
