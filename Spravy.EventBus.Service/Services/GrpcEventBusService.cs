using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using Spravy.Service.Extensions;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Service.Services;

public class GrpcEventBusService : EventBusServiceBase
{
    private readonly IMapper mapper;
    private readonly EventStorage eventStorage;
    private readonly ILogger<GrpcEventBusService> logger;

    public GrpcEventBusService(IMapper mapper, EventStorage eventStorage, ILogger<GrpcEventBusService> logger)
    {
        this.mapper = mapper;
        this.eventStorage = eventStorage;
        this.logger = logger;
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

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var eventValues = await WaitAnyEventAsync(eventIds, context.CancellationToken);

            if (!eventValues.IsSuccess)
            {
                return;
            }

            foreach (var eventValue in eventValues.Value)
            {
                var reply = new SubscribeEventsReply
                {
                    EventId = mapper.Map<ByteString>(eventValue.Id),
                    Content = ByteString.CopyFrom(eventValue.Content),
                };

                await responseStream.WriteAsync(reply);
            }
        }
    }

    public override async Task<PublishEventReply> PublishEvent(PublishEventRequest request, ServerCallContext context)
    {
        await eventStorage.AddEventAsync(mapper.Map<Guid>(request.EventId), request.Content.ToByteArray());

        return new PublishEventReply();
    }

    private async ValueTask<IsSuccessValue<IEnumerable<EventValue>>> WaitAnyEventAsync(
        Guid[] eventIds,
        CancellationToken cancellationToken
    )
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var eventValue = await eventStorage.PushEventAsync(eventIds);
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            return new IsSuccessValue<IEnumerable<EventValue>>(eventValue);
        }

        return new IsSuccessValue<IEnumerable<EventValue>>();
    }
}