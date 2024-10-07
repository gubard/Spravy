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

    [ObservableProperty]
    private string icon = string.Empty;

    public PlannedToDoItemSettingsViewModel(ToDoItemEntityNotify item, IToDoService toDoService)
    {
        Item = item;
        this.toDoService = toDoService;
        DueDate = item.DueDate;
        IsRequiredCompleteInDueDate = Item.IsRequiredCompleteInDueDate;
        ChildrenType = item.ChildrenType;
        Icon = item.Icon;
    }

    private ToDoItemEntityNotify Item { get; }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(Item.Id, ChildrenType, ct)
            .IfSuccessAsync(() => toDoService.UpdateToDoItemDueDateAsync(Item.Id, DueDate, ct), ct)
            .IfSuccessAsync(() => toDoService.UpdateIconAsync(Item.Id, Icon, ct), ct)
            .IfSuccessAsync(
                () =>
                    toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                        Item.Id,
                        IsRequiredCompleteInDueDate,
                        ct
                    ),
                ct
            );
    }

    public Result UpdateItemUi()
    {
        Item.IsRequiredCompleteInDueDate = IsRequiredCompleteInDueDate;
        Item.ChildrenType = ChildrenType;
        Item.DueDate = DueDate;
        Item.Icon = Icon;

        return Result.Success;
    }
}
