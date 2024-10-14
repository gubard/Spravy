namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ValueToDoItemSettingsViewModel : ViewModelBase, IEditToDoItems
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private ToDoItemChildrenType childrenType;

    [ObservableProperty]
    private string icon = string.Empty;

    public ValueToDoItemSettingsViewModel(
        ToDoItemEntityNotify item,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.toDoService = toDoService;
        Item = item;
        Icon = item.Icon;
        ChildrenType = item.ChildrenType;
    }

    public ToDoItemEntityNotify Item { get; }

    public EditToDoItems GetEditToDoItems()
    {
        return new EditToDoItems()
            .SetIds(new[] { Item.Id })
            .SetChildrenType(new(ChildrenType))
            .SetIcon(new(Icon));
    }

    public Result UpdateItemUi()
    {
        Item.ChildrenType = ChildrenType;
        Item.Icon = Icon;

        return Result.Success;
    }
}
