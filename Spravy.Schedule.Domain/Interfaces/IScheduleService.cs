using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    Task AddTimerAsync(AddTimerParameters parameters);
    Task<IEnumerable<TimerItem>> GetListTimesAsync();
    Task RemoveTimerAsync(Guid id);
}