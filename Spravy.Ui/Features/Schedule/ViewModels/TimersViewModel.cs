namespace Spravy.Ui.Features.Schedule.ViewModels;

public class TimersViewModel : NavigatableViewModelBase, IRefresh
{
    private readonly IScheduleService scheduleService;

    public TimersViewModel(IScheduleService scheduleService)
        : base(true)
    {
        this.scheduleService = scheduleService;
    }

    public AvaloniaList<TimerItemNotify> Timers { get; } = new();

    public override string ViewId
    {
        get => $"{TypeCache<TimersViewModel>.Type}";
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return scheduleService
            .GetTimersAsync(ct)
            .IfSuccessAsync(
                x =>
                    this.PostUiBackground(
                        () => Timers.UpdateUi(x.Select(y => y.ToTimerItemNotify())),
                        ct
                    ),
                ct
            );
    }
}
