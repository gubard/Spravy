namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemDayOfMonthSelectorViewModel(
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        SelectedDays = new();

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public AvaloniaList<int> SelectedDays { get; }
    public SpravyCommand InitializedCommand { get; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            ToDoItemId,
            new(SelectedDays.Select(x => (byte)x).ToArray()),
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return toDoService
            .GetMonthlyPeriodicityAsync(ToDoItemId, ct)
            .IfSuccessAsync(
                monthlyPeriodicity =>
                    this.InvokeUiBackgroundAsync(() =>
                    {
                        SelectedDays.AddRange(
                            monthlyPeriodicity.Days.Select(x => (int)x).ToArray()
                        );

                        return Result.Success;
                    }),
                ct
            );
    }
}
