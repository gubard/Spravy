namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public RandomizeChildrenOrderViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService
    ) : base(editItem, editItems)
    {
        this.toDoService = toDoService;
    }

    public override string ViewId => TypeCache<RandomizeChildrenOrderViewModel>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.RandomizeChildrenOrderIndexAsync(ResultCurrentIds, ct);
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}