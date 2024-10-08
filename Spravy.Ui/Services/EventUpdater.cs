namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private readonly IScheduleService scheduleService;
    private readonly IUiApplicationService uiApplicationService;
    private readonly IToDoService toDoService;
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private CancellationTokenSource cancellationTokenSource = new();

    public EventUpdater(
        IScheduleService scheduleService,
        ISpravyNotificationManager spravyNotificationManager,
        IToDoService toDoService,
        IUiApplicationService uiApplicationService
    )
    {
        this.scheduleService = scheduleService;
        this.spravyNotificationManager = spravyNotificationManager;
        this.toDoService = toDoService;
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
                await scheduleService
                    .UpdateEventsAsync(ct)
                    .IfSuccessAsync(
                        isUpdated =>
                        {
                            if (!isUpdated)
                            {
                                return Result.AwaitableSuccess;
                            }

                            return spravyNotificationManager
                                .ShowAsync(new TextLocalization("Notification.UpdateSchedule"), ct)
                                .IfSuccessAsync(() => toDoService.UpdateEventsAsync(ct), ct)
                                .IfSuccessAsync(
                                    x =>
                                    {
                                        if (!x)
                                        {
                                            return uiApplicationService.RefreshCurrentViewAsync(ct);
                                        }

                                        return spravyNotificationManager
                                            .ShowAsync(
                                                new TextLocalization(
                                                    "Notification.UpdateToDoEvents"
                                                ),
                                                ct
                                            )
                                            .IfSuccessAsync(
                                                () =>
                                                    uiApplicationService.RefreshCurrentViewAsync(
                                                        ct
                                                    ),
                                                ct
                                            );
                                    },
                                    ct
                                );
                        },
                        ct
                    )
                    .IfErrorsAsync(
                        errors =>
                            spravyNotificationManager.ShowAsync(
                                errors
                                    .Select(x => $"{x.Id}{Environment.NewLine}{x.Message}")
                                    .JoinString(Environment.NewLine),
                                ct
                            ),
                        ct
                    );
            }
            catch (Exception e)
            {
                await spravyNotificationManager.ShowAsync(e.Message, ct);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}
