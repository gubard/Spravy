using Google.Protobuf.WellKnownTypes;
using Spravy.Schedule.Domain.Mapper.Mappers;
using Spravy.Schedule.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcRouterRouterScheduleService : ScheduleService.ScheduleServiceBase
{
    private readonly IScheduleService scheduleService;
    private readonly ISerializer serializer;

    public GrpcRouterRouterScheduleService(IScheduleService scheduleService, ISerializer serializer)
    {
        this.scheduleService = scheduleService;
        this.serializer = serializer;
    }

    public override Task<UpdateEventsReply> UpdateEvents(UpdateEventsRequest request, ServerCallContext context)
    {
        return scheduleService.UpdateEventsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                value => new UpdateEventsReply
                {
                    IsUpdated = value,
                },
                context.CancellationToken
            );
    }

    public override Task<AddTimerReply> AddTimer(AddTimerRequest request, ServerCallContext context)
    {
        return scheduleService.AddTimerAsync(request.Items.Select(x => x.ToAddTimerParameters()).ToArray(), context.CancellationToken).HandleAsync<AddTimerReply>(serializer, context.CancellationToken);
    }

    public override async Task<RemoveTimerReply> RemoveTimer(RemoveTimerRequest request, ServerCallContext context)
    {
        await scheduleService.RemoveTimerAsync(request.Id.ToGuid(), context.CancellationToken);

        return new();
    }

    public override Task<GetTimersReply> GetTimers(GetTimersRequest request, ServerCallContext context)
    {
        return scheduleService.GetTimersAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                items =>
                {
                    var reply = new GetTimersReply();
                    reply.Items.AddRange(items.ToTimerItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }
}