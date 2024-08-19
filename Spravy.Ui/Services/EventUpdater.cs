namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private readonly IScheduleService scheduleService;
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private CancellationTokenSource cancellationTokenSource = new();

    public EventUpdater(
        IScheduleService scheduleService,
        ISpravyNotificationManager spravyNotificationManager
    )
    {
        this.scheduleService = scheduleService;
        this.spravyNotificationManager = spravyNotificationManager;
    }

    public void Start()
    {
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
            await scheduleService
                .UpdateEventsAsync(ct)
                .IfSuccessAsync(
                    isUpdated =>
                    {
                        if (isUpdated)
                        {
                            return spravyNotificationManager.ShowAsync(
                                new TextLocalization("Notification.UpdateSchedule"),
                                ct
                            );
                        }

                        return Result.AwaitableSuccess;
                    },
                    ct
                );

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}
