using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    ValueTask<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken);
    ValueTask<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken);
    ValueTask<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken);
}