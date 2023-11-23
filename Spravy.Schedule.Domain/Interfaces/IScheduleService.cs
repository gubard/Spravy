using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    Task AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken);
    Task<IEnumerable<TimerItem>> GetListTimesAsync(CancellationToken cancellationToken);
    Task RemoveTimerAsync(Guid id, CancellationToken cancellationToken);
}