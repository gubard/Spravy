namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    Cvtar ShowAsync<TView>(CancellationToken ct) where TView : notnull;

    Cvtar ShowAsync(object view, CancellationToken ct);
}