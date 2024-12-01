namespace Spravy.Schedule.Domain.Interfaces;

public interface IScheduleService
{
    Cvtar AddTimerAsync(ReadOnlyMemory<AddTimerParameters> parameters, CancellationToken ct);
    Cvtar RemoveTimerAsync(Guid id, CancellationToken ct);
    ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct);

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetTimersAsync(CancellationToken ct);
}