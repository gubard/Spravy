namespace Spravy.Ui.Interfaces;

public interface IAudioService
{
    Cvtar PlayCompleteAsync(CancellationToken ct);
    Cvtar PlayNotificationAsync(CancellationToken ct);
}
