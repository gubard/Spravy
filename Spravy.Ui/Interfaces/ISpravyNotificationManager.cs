namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken cancellationToken) where TView : notnull;
    ConfiguredValueTaskAwaitable<Result> ShowAsync(object view, CancellationToken cancellationToken);
}