namespace Spravy.Schedule.Service.Services;

[Authorize]
public class GrpcScheduleService : ScheduleService.ScheduleServiceBase
{
    private readonly IScheduleService scheduleService;
    private readonly ISerializer serializer;

    public GrpcScheduleService(IScheduleService scheduleService, ISerializer serializer)
    {
        this.scheduleService = scheduleService;
        this.serializer = serializer;
    }

    public override Task<UpdateEventsReply> UpdateEvents(
        UpdateEventsRequest request,
        ServerCallContext context
    )
    {
        return scheduleService
            .UpdateEventsAsync(context.CancellationToken)
            .HandleAsync(
                serializer,
                isUpdated =>
                {
                    var reply = new UpdateEventsReply { IsUpdated = isUpdated };

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override async Task<AddTimerReply> AddTimer(
        AddTimerRequest request,
        ServerCallContext context
    )
    {
        var parameters = request.Items.Select(x => x.ToAddTimerParameters()).ToArray();
        await scheduleService.AddTimerAsync(parameters, context.CancellationToken);

        return new();
    }

    public override async Task<RemoveTimerReply> RemoveTimer(
        RemoveTimerRequest request,
        ServerCallContext context
    )
    {
        var id = request.Id.ToGuid();
        await scheduleService.RemoveTimerAsync(id, context.CancellationToken);

        return new();
    }

    public override Task<GetTimersReply> GetTimers(
        GetTimersRequest request,
        ServerCallContext context
    )
    {
        return scheduleService
            .GetTimersAsync(context.CancellationToken)
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
