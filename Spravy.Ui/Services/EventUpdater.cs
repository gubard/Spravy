namespace Spravy.Ui.Services;

public class EventUpdater : IEventUpdater
{
    private readonly IAudioService audioService;
    private readonly IScheduleService scheduleService;
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private readonly IToDoService toDoService;
    private readonly IUiApplicationService uiApplicationService;
    private CancellationTokenSource cancellationTokenSource = new();

    public EventUpdater(
        IScheduleService scheduleService,
        ISpravyNotificationManager spravyNotificationManager,
        IToDoService toDoService,
        IUiApplicationService uiApplicationService,
        IAudioService audioService
    )
    {
        this.scheduleService = scheduleService;
        this.spravyNotificationManager = spravyNotificationManager;
        this.toDoService = toDoService;
        this.uiApplicationService = uiApplicationService;
        this.audioService = audioService;
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
                await scheduleService.UpdateEventsAsync(ct)
                   .IfSuccessAsync(
                        isUpdated =>
                        {
                            if (!isUpdated)
                            {
                                return Result.AwaitableSuccess;
                            }

                            return spravyNotificationManager.Show(new TextLocalization("Lang.UpdateSchedule"))
                               .IfSuccessAsync(() => audioService.PlayNotificationAsync(ct), ct)
                               .IfSuccessAsync(() => toDoService.UpdateEventsAsync(ct), ct)
                               .IfSuccessAsync(
                                    x =>
                                    {
                                        if (!x)
                                        {
                                            return uiApplicationService.RefreshCurrentViewAsync(ct);
                                        }

                                        return spravyNotificationManager
                                           .Show(new TextLocalization("lang.UpdateToDoEvents"))
                                           .IfSuccessAsync(() => audioService.PlayNotificationAsync(ct), ct)
                                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct);
                                    },
                                    ct
                                );
                        },
                        ct
                    )
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