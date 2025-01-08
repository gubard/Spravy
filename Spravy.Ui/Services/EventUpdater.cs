namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private readonly IUiApplicationService uiApplicationService;
    private CancellationTokenSource cancellationTokenSource = new();

    public EventUpdater(
        ISpravyNotificationManager spravyNotificationManager,
        IUiApplicationService uiApplicationService
    )
    {
        this.spravyNotificationManager = spravyNotificationManager;
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
                        errors => spravyNotificationManager.Show(
                            errors.Select(x => $"{x.Id}{Environment.NewLine}{x.Message}")
                               .JoinString(Environment.NewLine)
                        ),
                        ct
                    );
            }
            catch (Exception e)
            {
                spravyNotificationManager.Show(e.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}