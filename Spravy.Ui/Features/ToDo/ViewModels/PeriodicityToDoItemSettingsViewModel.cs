namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private IApplySettings? periodicity;

    [ObservableProperty]
    private ToDoItemEntityNotify? item;

    public PeriodicityToDoItemSettingsViewModel(
        IToDoService toDoService,
        IServiceFactory serviceFactory,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IToDoUiService toDoUiService
    )
    {
        this.toDoService = toDoService;
        this.serviceFactory = serviceFactory;
        this.toDoUiService = toDoUiService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(
                i =>
                    toDoService
                        .UpdateToDoItemChildrenTypeAsync(i.Id, i.ChildrenType, ct)
                        .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemDueDateAsync(i.Id, i.DueDate, ct),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                                    i.Id,
                                    i.IsRequiredCompleteInDueDate,
                                    ct
                                ),
                            ct
                        )
                        .IfSuccessAsync(
                            () =>
                                toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
                                    i.Id,
                                    i.TypeOfPeriodicity,
                                    ct
                                ),
                            ct
                        )
                        .IfSuccessAsync(() => Periodicity.ThrowIfNull().ApplySettingsAsync(ct), ct),
                ct
            );
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return Item.IfNotNull(nameof(Item))
            .IfSuccessAsync(
                i =>
                    toDoUiService
                        .UpdateItemAsync(i, ct)
                        .IfSuccessAsync(
                            () =>
                                this.PostUiBackground(
                                    () =>
                                    {
                                        Periodicity = i.TypeOfPeriodicity switch
                                        {
                                            TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                                            TypeOfPeriodicity.Weekly
                                                => serviceFactory
                                                    .CreateService<ToDoItemDayOfWeekSelectorViewModel>()
                                                    .Case(y => y.ToDoItemId = i.Id),
                                            TypeOfPeriodicity.Monthly
                                                => serviceFactory
                                                    .CreateService<ToDoItemDayOfMonthSelectorViewModel>()
                                                    .Case(y => y.ToDoItemId = i.Id),
                                            TypeOfPeriodicity.Annually
                                                => serviceFactory
                                                    .CreateService<ToDoItemDayOfYearSelectorViewModel>()
                                                    .Case(y => y.ToDoItemId = i.Id),
                                            _
                                                => throw new ArgumentOutOfRangeException(
                                                    nameof(TypeOfPeriodicity),
                                                    i.TypeOfPeriodicity,
                                                    null
                                                ),
                                        };

                                        return Result.Success;
                                    },
                                    ct
                                ),
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
