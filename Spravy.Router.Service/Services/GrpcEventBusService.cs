using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using Spravy.Service.Extensions;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcEventBusService : EventBusService.EventBusServiceBase
{
    private readonly IMapper mapper;
    private readonly IEventBusService eventBusService;
    private readonly ILogger<GrpcEventBusService> logger;

    public GrpcEventBusService(IMapper mapper, ILogger<GrpcEventBusService> logger, IEventBusService eventBusService)
    {
        this.mapper = mapper;
        this.logger = logger;
        this.eventBusService = eventBusService;
    }

    public override async Task SubscribeEvents(
        SubscribeEventsRequest request,
        IServerStreamWriter<SubscribeEventsReply> responseStream,
        ServerCallContext context
    )
    {
        var eventIds = mapper.Map<Guid[]>(request.EventIds);
        var userId = context.GetHttpContext().GetUserId();
        logger.LogInformation("{UserId} subscribe events {EventIds}", userId, eventIds);
        var events = eventBusService.SubscribeEventsAsync(eventIds, context.CancellationToken);

        await foreach (var @event in events)
        {
            var subscribeEventsReply = mapper.Map<SubscribeEventsReply>(@event);
            await responseStream.WriteAsync(subscribeEventsReply, context.CancellationToken);
        }
    }

    public override async Task<PublishEventReply> PublishEvent(PublishEventRequest request, ServerCallContext context)
    {
        var userId = context.GetHttpContext().GetUserId();
        var id = mapper.Map<Guid>(request.EventId);
        logger.LogInformation("{UserId} push event {Id}", userId, id);
        await eventBusService.PublishEventAsync(id, request.Content.ToByteArray());

        return new PublishEventReply();
    }

    public override async Task<GetEventsReply> GetEvents(GetEventsRequest request, ServerCallContext context)
    {
        var userId = context.GetHttpContext().GetUserId();
        var eventIds = mapper.Map<Guid[]>(request.EventIds);
        logger.LogInformation("{UserId} get events {EventIds}", userId, eventIds);
        var eventValues = await eventBusService.GetEventsAsync(eventIds);
        var reply = new GetEventsReply();
        reply.Events.AddRange(mapper.Map<IEnumerable<Event>>(eventValues));

        return reply;
    }
}