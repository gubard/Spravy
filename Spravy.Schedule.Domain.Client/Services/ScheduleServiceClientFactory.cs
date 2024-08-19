namespace Spravy.Schedule.Domain.Client.Services;

public class ScheduleServiceClientFactory
    : IFactory<ChannelBase, ScheduleService.ScheduleServiceClient>
{
    public Result<ScheduleService.ScheduleServiceClient> Create(ChannelBase key)
    {
        return new(new ScheduleService.ScheduleServiceClient(key));
    }
}
