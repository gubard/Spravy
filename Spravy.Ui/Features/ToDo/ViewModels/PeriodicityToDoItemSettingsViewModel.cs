namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IApplySettings? periodicity;

    public PeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService,
        IViewFactory viewFactory
    )
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        this.viewFactory = viewFactory;
        Item = item;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public ToDoItemEntityNotify Item { get; }
    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Item.Id, Item.ChildrenType, ct)
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemDueDateAsync(Item.Id, Item.DueDate, ct),
                ct
            )
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Item.Id,
                        Item.IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
                        Item.Id,
                        Item.TypeOfPeriodicity,
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(() => Periodicity.ThrowIfNull().ApplySettingsAsync(ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoUiService
            .UpdateItemAsync(Item, ct)
            .IfSuccessAsync(
                () =>
                    this.PostUiBackground(
                        () =>
                        {
                            Periodicity = Item.TypeOfPeriodicity switch
                            {
                                TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                                TypeOfPeriodicity.Weekly
                                    => viewFactory.CreateToDoItemDayOfWeekSelectorViewModel(Item),
                                TypeOfPeriodicity.Monthly
                                    => viewFactory.CreateToDoItemDayOfMonthSelectorViewModel(Item),
                                TypeOfPeriodicity.Annually
                                    => viewFactory.CreateToDoItemDayOfYearSelectorViewModel(Item),
                                _
                                    => throw new ArgumentOutOfRangeException(
                                        nameof(TypeOfPeriodicity),
                                        Item.TypeOfPeriodicity,
                                        null
                                    ),
                            };

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }
}
