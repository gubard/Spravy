using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    Task<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken);
    Task<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken);
    Task<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken);
}