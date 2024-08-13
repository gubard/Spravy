namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IApplySettings periodicity;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private TypeOfPeriodicity typeOfPeriodicity;

    public PeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IViewFactory viewFactory
    )
    {
        Item = item;
        this.toDoService = toDoService;
        this.viewFactory = viewFactory;
        isRequiredCompleteInDueDate = item.IsRequiredCompleteInDueDate;
        childrenType = item.ChildrenType;
        dueDate = item.DueDate;
        typeOfPeriodicity = item.TypeOfPeriodicity;
        Periodicity = CreatePeriodicity();
        PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Item.Id, ChildrenType, ct)
            .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Item.Id, DueDate, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Item.Id,
                        IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
                        Item.Id,
                        TypeOfPeriodicity,
                        ct
                    ),
                ct
            )
            .IfSuccessAsync(() => Periodicity.ApplySettingsAsync(ct), ct);
    }

    private IApplySettings CreatePeriodicity()
    {
        return TypeOfPeriodicity switch
        {
            TypeOfPeriodicity.Daily => new EmptyApplySettings(),
            TypeOfPeriodicity.Weekly => viewFactory.CreateToDoItemDayOfWeekSelectorViewModel(Item),
            TypeOfPeriodicity.Monthly
                => viewFactory.CreateToDoItemDayOfMonthSelectorViewModel(Item),
            TypeOfPeriodicity.Annually
                => viewFactory.CreateToDoItemDayOfYearSelectorViewModel(Item),
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(TypeOfPeriodicity),
                    TypeOfPeriodicity,
                    null
                ),
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TypeOfPeriodicity))
        {
            Periodicity = CreatePeriodicity();
        }
    }
}
