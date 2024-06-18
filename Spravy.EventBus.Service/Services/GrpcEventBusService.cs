using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Mapper.Mappers;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using Spravy.Service.Extensions;

namespace Spravy.EventBus.Service.Services;

[Authorize]
public class GrpcEventBusService : EventBusService.EventBusServiceBase
{
    private readonly EventStorage eventStorage;
    private readonly ILogger<GrpcEventBusService> logger;

    public GrpcEventBusService( EventStorage eventStorage, ILogger<GrpcEventBusService> logger)
    {
        this.eventStorage = eventStorage;
        this.logger = logger;
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

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var eventValues = await WaitAnyEventAsync(eventIds, context.CancellationToken);

            if (!eventValues.IsSuccess)
            {
                return;
            }

            foreach (var eventValue in eventValues.Value.ToArray())
            {
                var reply = new SubscribeEventsReply
                {
                    EventId = eventValue.Id.ToByteString(),
                    Content = ByteString.CopyFrom(eventValue.Content.Span),
                };

                logger.LogInformation("Send event {Id}", eventValue.Id);
                await responseStream.WriteAsync(reply);
            }
        }
    }

    public override async Task<PublishEventReply> PublishEvent(PublishEventRequest request, ServerCallContext context)
    {
        var userId = context.GetHttpContext().GetUserId();
        var id = request.EventId.ToGuid();
        logger.LogInformation("{UserId} push event {Id}", userId, id);
        await eventStorage.AddEventAsync(id, request.Content.ToByteArray(), context.CancellationToken);

        return new();
    }

    public override async Task<GetEventsReply> GetEvents(GetEventsRequest request, ServerCallContext context)
    {
        var userId = context.GetHttpContext().GetUserId();
        var eventIds = request.EventIds.ToGuid();
        logger.LogInformation("{UserId} get events {EventIds}", userId, eventIds);
        var eventValues = await eventStorage.PushEventAsync(eventIds);
        var reply = new GetEventsReply();
        reply.Events.AddRange(eventValues.ToEvent().ToArray());

        return reply;
    }

    private async ValueTask<IsSuccessValue<ReadOnlyMemory<EventValue>>> WaitAnyEventAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var eventValue = await eventStorage.PushEventAsync(eventIds);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            return new(eventValue);
        }

        return new();
    }
}