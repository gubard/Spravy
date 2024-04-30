using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcRouterRouterScheduleService : ScheduleService.ScheduleServiceBase
{
    private readonly IMapper mapper;
    private readonly IScheduleService scheduleService;

    public GrpcRouterRouterScheduleService(IScheduleService scheduleService, IMapper mapper)
    {
        this.scheduleService = scheduleService;
        this.mapper = mapper;
    }

    public override async Task<AddTimerReply> AddTimer(AddTimerRequest request, ServerCallContext context)
    {
        var parameters = mapper.Map<AddTimerParameters>(request.Parameters);
        await scheduleService.AddTimerAsync(parameters, context.CancellationToken);

        return new();
    }

    public override async Task<RemoveTimerReply> RemoveTimer(RemoveTimerRequest request, ServerCallContext context)
    {
        var id = mapper.Map<Guid>(request.Id);
        await scheduleService.RemoveTimerAsync(id, context.CancellationToken);

        return new();
    }

    public override async Task<GetListTimesReply> GetListTimes(GetListTimesRequest request, ServerCallContext context)
    {
        var items = await scheduleService.GetListTimesAsync(context.CancellationToken);
        var reply = new GetListTimesReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<TimerItemGrpc>>(items));

        return reply;
    }
}