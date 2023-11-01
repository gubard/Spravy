using Spravy.Domain.Extensions;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Ui.Design.Services;

public class ScheduleServiceDesign : IScheduleService
{
    private readonly IEnumerable<TimerItem> timers;

    public ScheduleServiceDesign(IEnumerable<TimerItem> timers)
    {
        this.timers = timers;
    }

    public Task AddTimerAsync(AddTimerParameters parameters)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        return timers.ToTaskResult();
    }

    public Task RemoveTimerAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}