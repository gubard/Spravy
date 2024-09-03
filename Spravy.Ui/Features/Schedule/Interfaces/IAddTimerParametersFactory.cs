namespace Spravy.Ui.Features.Schedule.Interfaces;

public interface IAddTimerParametersFactory
{
    ConfiguredValueTaskAwaitable<Result<AddTimerParameters>> CreateAddTimerParametersAsync(
        CancellationToken ct
    );
}
