namespace Spravy.Ui.Services;

public class AudioService : IAudioService
{
    public Cvtar PlayCompleteAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
