namespace Spravy.Ui.Features.ToDo.ViewModels;

public class EditDescriptionViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public EditDescriptionViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        EditDescriptionContentViewModel content,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        Content = content;
        this.toDoService = toDoService;

        if (!editItem.TryGetValue(out var item))
        {
            return;
        }

        Content.DescriptionType = item.DescriptionType;
        Content.Description = item.Description;
    }

    public EditDescriptionContentViewModel Content { get; }

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

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return toDoService.EditToDoItemsAsync(
            new EditToDoItems()
                .SetIds(ResultIds)
                .SetDescriptionType(new(Content.DescriptionType))
                .SetDescription(new(Content.Description)),
            ct
        );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }
}
