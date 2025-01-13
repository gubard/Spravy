namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private CancellationTokenSource cancellationTokenSource = new();

    public void Start()
    {
        Stop();
        _ = StartCore();
    }

    public void Stop()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    private ValueTask StartCore()
    {
        return ValueTask.CompletedTask;
    }
}