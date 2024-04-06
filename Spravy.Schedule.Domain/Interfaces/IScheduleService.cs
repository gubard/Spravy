using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(
        CancellationToken cancellationToken
    );

    ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken);
}