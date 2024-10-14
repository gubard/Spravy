namespace Spravy.Ui.Features.Schedule.Interfaces;

public interface IAddTimerParameters
{
    ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<AddTimerParameters>>
    > GetAddTimerParametersAsync(CancellationToken ct);
}
