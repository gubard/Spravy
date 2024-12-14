namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemSettingsViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public ToDoItemSettingsViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService,
        EditToDoItemViewModel editToDoItemViewModel
    ) : base(editItem, editItems)
    {
        this.toDoService = toDoService;
        EditToDoItemViewModel = editToDoItemViewModel;
    }

    public EditToDoItemViewModel EditToDoItemViewModel { get; }

    public override string ViewId => TypeCache<ToDoItemSettingsViewModel>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(EditToDoItemViewModel.GetEditToDoItems().SetIds(ResultIds), ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return EditToDoItemViewModel.LoadStateAsync(ct);
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return EditToDoItemViewModel.SaveStateAsync(ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}