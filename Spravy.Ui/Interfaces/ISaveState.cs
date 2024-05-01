namespace Spravy.Ui.Interfaces;

public interface ISaveState
{
    ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken);
}