namespace Spravy.Ui.Features.Schedule.ViewModels;

public class TimersViewModel : NavigatableViewModelBase
{
    private readonly IScheduleService scheduleService;

    public TimersViewModel(
        IScheduleService scheduleService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        this.scheduleService = scheduleService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }
    public AvaloniaList<TimerItemNotify> Timers { get; } = new();

    private Cvtar InitializedAsync(CancellationToken ct)
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
}
