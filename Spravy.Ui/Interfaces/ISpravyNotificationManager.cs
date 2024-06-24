namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken ct) where TView : notnull;
    ConfiguredValueTaskAwaitable<Result> ShowAsync(object view, CancellationToken ct);
}