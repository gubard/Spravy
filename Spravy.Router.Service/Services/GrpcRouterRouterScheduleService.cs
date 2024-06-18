using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Mappers;
using Spravy.Schedule.Protos;
using Spravy.Service.Extensions;

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

    public override async Task<AddTimerReply> AddTimer(AddTimerRequest request, ServerCallContext context)
    {
        var parameters = request.Parameters.ToAddTimerParameters();
        await scheduleService.AddTimerAsync(parameters, context.CancellationToken);

        return new();
    }

    public override async Task<RemoveTimerReply> RemoveTimer(RemoveTimerRequest request, ServerCallContext context)
    {
        var id = request.Id.ToGuid();
        await scheduleService.RemoveTimerAsync(id, context.CancellationToken);

        return new();
    }

    public override Task<GetListTimesReply> GetListTimes(GetListTimesRequest request, ServerCallContext context)
    {
        return scheduleService.GetListTimesAsync(context.CancellationToken)
           .HandleAsync(serializer, items =>
            {
                var reply = new GetListTimesReply();
                reply.Items.AddRange(items.ToTimerItemGrpc().ToArray());

                return reply;
            });
    }
}