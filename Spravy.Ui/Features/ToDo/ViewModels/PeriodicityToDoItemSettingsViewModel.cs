namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PeriodicityToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
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

    [ObservableProperty]
    private uint remindDaysBefore;

    private readonly EmptyEditToDoItems empty;
    private readonly ToDoItemDayOfWeekSelectorViewModel dayOfWeekSelector;
    private readonly ToDoItemDayOfMonthSelectorViewModel dayOfMonthSelector;
    private readonly ToDoItemDayOfYearSelectorViewModel dayOfYearSelector;

    public PeriodicityToDoItemSettingsViewModel(ToDoItemEntityNotify item, IViewFactory viewFactory)
    {
        Item = item;
        isRequiredCompleteInDueDate = item.IsRequiredCompleteInDueDate;
        childrenType = item.ChildrenType;
        dueDate = item.DueDate;
        typeOfPeriodicity = item.TypeOfPeriodicity;
        RemindDaysBefore = item.RemindDaysBefore;
        empty = new();
        dayOfWeekSelector = viewFactory.CreateToDoItemDayOfWeekSelectorViewModel(Item);
        dayOfMonthSelector = viewFactory.CreateToDoItemDayOfMonthSelectorViewModel(Item);
        dayOfYearSelector = viewFactory.CreateToDoItemDayOfYearSelectorViewModel(Item);
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
            .SetIds(new[] { Item.Id })
            .SetRemindDaysBefore(new(RemindDaysBefore));
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
        Item.RemindDaysBefore = RemindDaysBefore;

        return Periodicity.UpdateItemUi();
    }
}
