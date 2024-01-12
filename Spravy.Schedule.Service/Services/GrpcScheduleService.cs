using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Service.Services;

[Authorize]
public class GrpcScheduleService : ScheduleService.ScheduleServiceBase
{
    private readonly IScheduleService scheduleService;
    private readonly IMapper mapper;

    public GrpcScheduleService(IScheduleService scheduleService, IMapper mapper)
    {
        this.scheduleService = scheduleService;
        this.mapper = mapper;
    }

    public override async Task<AddTimerReply> AddTimer(AddTimerRequest request, ServerCallContext context)
    {
        var parameters = mapper.Map<AddTimerParameters>(request.Parameters);
        await scheduleService.AddTimerAsync(parameters, context.CancellationToken);

        return new AddTimerReply();
    }

    public override async Task<RemoveTimerReply> RemoveTimer(RemoveTimerRequest request, ServerCallContext context)
    {
        var id = mapper.Map<Guid>(request.Id);
        await scheduleService.RemoveTimerAsync(id, context.CancellationToken);

        return new RemoveTimerReply();
    }

    public override async Task<GetListTimesReply> GetListTimes(GetListTimesRequest request, ServerCallContext context)
    {
        var items = await scheduleService.GetListTimesAsync(context.CancellationToken);
        var reply = new GetListTimesReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<TimerItemGrpc>>(items));

        return reply;
    }
}