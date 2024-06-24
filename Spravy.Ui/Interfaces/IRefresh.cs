namespace Spravy.Ui.Interfaces;

public interface IRefresh
{
    ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct);
}