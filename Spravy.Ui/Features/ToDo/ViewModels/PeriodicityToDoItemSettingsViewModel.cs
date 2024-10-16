namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : IconViewModel, IToDoItemSettings
{
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

    private readonly EmptyEditToDoItems empty;
    private readonly ToDoItemDayOfWeekSelectorViewModel dayOfWeekSelector;
    private readonly ToDoItemDayOfMonthSelectorViewModel dayOfMonthSelector;
    private readonly ToDoItemDayOfYearSelectorViewModel dayOfYearSelector;

    public PeriodicityToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IViewFactory viewFactory,
        IObjectStorage objectStorage
    )
        : base(objectStorage)
    {
        Item = item;
        isRequiredCompleteInDueDate = item.IsRequiredCompleteInDueDate;
        childrenType = item.ChildrenType;
        dueDate = item.DueDate;
        typeOfPeriodicity = item.TypeOfPeriodicity;
        Icon = item.Icon;
        empty = new();
        dayOfWeekSelector = viewFactory.CreateToDoItemDayOfWeekSelectorViewModel(Item);
        dayOfMonthSelector = viewFactory.CreateToDoItemDayOfMonthSelectorViewModel(Item);
        dayOfYearSelector = viewFactory.CreateToDoItemDayOfYearSelectorViewModel(Item);
        Periodicity = CreatePeriodicity();
        PropertyChanged += OnPropertyChanged;
    }

    public ToDoItemEntityNotify Item { get; }
    public override string ViewId => TypeCache<PeriodicityToDoItemSettingsViewModel>.Type.Name;

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
            TypeOfPeriodicity.Daily => empty,
            TypeOfPeriodicity.Weekly => dayOfWeekSelector,
            TypeOfPeriodicity.Monthly => dayOfMonthSelector,
            TypeOfPeriodicity.Annually => dayOfYearSelector,
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
