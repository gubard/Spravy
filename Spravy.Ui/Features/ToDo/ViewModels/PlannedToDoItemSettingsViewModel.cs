namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class PlannedToDoItemSettingsViewModel : ViewModelBase, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private bool isRequiredCompleteInDueDate;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private DateOnly dueDate;

    public PlannedToDoItemSettingsViewModel(ToDoItemEntityNotify item, IToDoService toDoService)
    {
        Item = item;
        this.toDoService = toDoService;
        DueDate = item.DueDate;
        IsRequiredCompleteInDueDate = Item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
    }

    private ToDoItemEntityNotify Item { get; }

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
            );
    }
}
