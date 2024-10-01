namespace Spravy.Ui.Interfaces;

public interface IAudioService
{
    Cvtar PlayCompleteAsync(CancellationToken ct);
}
