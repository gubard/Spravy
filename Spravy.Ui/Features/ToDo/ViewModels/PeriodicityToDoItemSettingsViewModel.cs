namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    private readonly IToDoService toDoService;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private IEditToDoItems periodicity;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    [ObservableProperty]
    private TypeOfPeriodicity typeOfPeriodicity;

    [ObservableProperty]
    private string icon = string.Empty;

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
        Icon = item.Icon;
        Periodicity = CreatePeriodicity();
        PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return Periodicity
            .GetEditToDoItems()
            .SetIsRequiredCompleteInDueDate(new(IsRequiredCompleteInDueDate))
            .SetChildrenType(new(ChildrenType))
            .SetDueDate(new(DueDate))
            .SetTypeOfPeriodicity(new(TypeOfPeriodicity))
            .SetIcon(new(Icon))
            .SetIds(new[] { Item.Id });
    }

    private IEditToDoItems CreatePeriodicity()
    {
        return TypeOfPeriodicity switch
        {
            TypeOfPeriodicity.Daily => new EmptyEditToDoItems(),
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

    public Result UpdateItemUi()
    {
        Item.IsRequiredCompleteInDueDate = IsRequiredCompleteInDueDate;
        Item.ChildrenType = ChildrenType;
        Item.TypeOfPeriodicity = TypeOfPeriodicity;
        Item.DueDate = DueDate;
        Item.Icon = Icon;

        return Periodicity.UpdateItemUi();
    }
}
