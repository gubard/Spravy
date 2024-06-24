using System.Runtime.CompilerServices;
using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken ct);
}