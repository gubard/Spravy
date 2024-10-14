namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RandomizeChildrenOrderViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public RandomizeChildrenOrderViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        this.toDoService = toDoService;
    }

    public override string ViewId
    {
        get => $"{TypeCache<RandomizeChildrenOrderViewModel>.Type}";
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.RandomizeChildrenOrderIndexAsync(
            ResultItems.Select(x => x.CurrentId),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
