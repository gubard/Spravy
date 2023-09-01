using AutoMapper;
using Grpc.Core;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;
using static Spravy.Schedule.Protos.ScheduleService;

namespace Spravy.Schedule.Service.Services;

public class GrpcScheduleService : ScheduleServiceBase
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
        await scheduleService.AddTimerAsync(parameters);

        return new AddTimerReply();
    }

    public override async Task<RemoveTimerReply> RemoveTimer(RemoveTimerRequest request, ServerCallContext context)
    {
        var id = mapper.Map<Guid>(request.Id);
        await scheduleService.RemoveTimerAsync(id);

        return new RemoveTimerReply();
    }

    public override async Task<GetListTimesReply> GetListTimes(GetListTimesRequest request, ServerCallContext context)
    {
        var items = await scheduleService.GetListTimesAsync();
        var reply = new GetListTimesReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<TimerItemGrpc>>(items));

        return reply;
    }
}