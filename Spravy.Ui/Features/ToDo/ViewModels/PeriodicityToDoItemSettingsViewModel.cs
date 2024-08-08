namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel
    : ViewModelBase,
        IToDoChildrenTypeProperty,
        IToDoDueDateProperty,
        IToDoTypeOfPeriodicityProperty,
        IIsRequiredCompleteInDueDateProperty,
        IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IServiceFactory serviceFactory;

    [ObservableProperty]
    private IApplySettings? periodicity;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private TypeOfPeriodicity typeOfPeriodicity;

    public PeriodicityToDoItemSettingsViewModel(
        IToDoService toDoService,
        IServiceFactory serviceFactory,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        this.serviceFactory = serviceFactory;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        PropertyChanged += OnPropertyChanged;
    }

    public SpravyCommand InitializedCommand { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, ct)
            .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Id, DueDate, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Id,
                        IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(
                () => toDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, TypeOfPeriodicity, ct),
                ct
            )
            .IfSuccessAsync(() => Periodicity.ThrowIfNull().ApplySettingsAsync(ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken ct)
    {
        return toDoService
            .GetPeriodicityToDoItemSettingsAsync(Id, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            ChildrenType = setting.ChildrenType;
                            DueDate = setting.DueDate;
                            TypeOfPeriodicity = setting.TypeOfPeriodicity;
                            IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;

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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TypeOfPeriodicity))
        {
            Periodicity = TypeOfPeriodicity switch
            {
                TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                TypeOfPeriodicity.Weekly
                    => serviceFactory
                        .CreateService<ToDoItemDayOfWeekSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Monthly
                    => serviceFactory
                        .CreateService<ToDoItemDayOfMonthSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                TypeOfPeriodicity.Annually
                    => serviceFactory
                        .CreateService<ToDoItemDayOfYearSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                _
                    => throw new ArgumentOutOfRangeException(
                        nameof(TypeOfPeriodicity),
                        TypeOfPeriodicity,
                        null
                    ),
            };
        }
    }
}
