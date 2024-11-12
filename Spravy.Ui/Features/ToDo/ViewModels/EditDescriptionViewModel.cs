namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class EditDescriptionViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    [ObservableProperty]
    private DescriptionType descriptionType;

    [ObservableProperty]
    private string description = string.Empty;

    public EditDescriptionViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        this.toDoService = toDoService;

        if (!editItem.TryGetValue(out var item))
        {
            return;
        }

        descriptionType = item.DescriptionType;
        description = item.Description;
    }

    public override string ViewId
    {
        get => $"{TypeCache<EditDescriptionViewModel>.Type}";
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

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(
            new EditToDoItems()
                .SetIds(ResultIds)
                .SetDescriptionType(new(DescriptionType))
                .SetDescription(new(Description)),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
