using Grpc.Core;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Domain.Client.Services;

public class ScheduleServiceClientFactory
    : IFactory<ChannelBase, ScheduleService.ScheduleServiceClient>
{
    public Result<ScheduleService.ScheduleServiceClient> Create(ChannelBase key)
    {
        return new(new ScheduleService.ScheduleServiceClient(key));
    }
}
