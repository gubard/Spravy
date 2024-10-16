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
