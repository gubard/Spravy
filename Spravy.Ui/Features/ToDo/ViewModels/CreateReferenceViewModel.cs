namespace Spravy.Ui.Features.ToDo.ViewModels;

public class CreateReferenceViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    public override string ViewId => TypeCache<CreateReferenceViewModel>.Type.Name;
    private readonly IToDoService toDoService;

    public CreateReferenceViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        ToDoItemSelectorViewModel toDoItemSelectorViewModel,
        IToDoService toDoService
    )
        : base(editItem, editItems)
    {
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
        this.toDoService = toDoService;
    }

    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

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
        if (ToDoItemSelectorViewModel.SelectedItem is null)
        {
            return ResultItems
                .ToResult()
                .IfSuccessForEach(x =>
                    new AddToDoItemOptions(
                        new(),
                        x.Name,
                        ToDoItemType.Reference,
                        x.Description,
                        x.DescriptionType,
                        x.Link.ToOptionUri(),
                        new(x.Id),
                        x.Icon,
                        x.Color.ToString()
                    ).ToResult()
                )
                .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct)
                .ToResultOnlyAsync();
        }

        return ResultItems
            .ToResult()
            .IfSuccessForEach(x =>
                new AddToDoItemOptions(
                    new(ToDoItemSelectorViewModel.SelectedItem.Id),
                    x.Name,
                    ToDoItemType.Reference,
                    x.Description,
                    x.DescriptionType,
                    x.Link.ToOptionUri(),
                    new(x.Id),
                    x.Icon,
                    x.Color.ToString()
                ).ToResult()
            )
            .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct)
            .ToResultOnlyAsync();
    }

    public Result UpdateItemUi()
    {
        return ToDoItemSelectorViewModel
            .GetSelectedItem()
            .IfSuccess(selectedItem =>
            {
                if (EditItem.TryGetValue(out var item))
                {
                    if (EditItems.IsEmpty)
                    {
                        item.Reference = selectedItem;
                    }
                    else
                    {
                        foreach (var i in EditItems.Span)
                        {
                            i.Reference = selectedItem;
                        }
                    }
                }
                else
                {
                    foreach (var i in EditItems.Span)
                    {
                        i.Reference = selectedItem;
                    }
                }

                return Result.Success;
            });
    }
}
