using Serilog;

namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private readonly IUiApplicationService uiApplicationService;
    private CancellationTokenSource cancellationTokenSource = new();

    public EventUpdater(
        IUiApplicationService uiApplicationService
    )
    {
        this.uiApplicationService = uiApplicationService;
    }

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

    private async ValueTask StartCore()
    {
        var ct = cancellationTokenSource.Token;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await uiApplicationService.RefreshCurrentViewAsync(ct)
                   .IfErrorsAsync(
                        errors =>
                        {
                            Log.Logger.Error("Event errors: {Errors}", errors);

                            return Result.Success;
                        },
                        ct
                    );
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Event exception");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}