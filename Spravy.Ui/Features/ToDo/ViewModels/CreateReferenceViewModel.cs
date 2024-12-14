namespace Spravy.Ui.Features.ToDo.ViewModels;

public class CreateReferenceViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IToDoService toDoService;

    public CreateReferenceViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        ToDoItemSelectorViewModel toDoItemSelectorViewModel,
        IToDoService toDoService
    ) : base(editItem, editItems)
    {
        ToDoItemSelectorViewModel = toDoItemSelectorViewModel;
        this.toDoService = toDoService;
    }

    public override string ViewId => TypeCache<CreateReferenceViewModel>.Name;

    public ToDoItemSelectorViewModel ToDoItemSelectorViewModel { get; }

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        if (ToDoItemSelectorViewModel.SelectedItem is null)
        {
            return ResultItems.ToResult()
               .IfSuccessForEach(
                    x => new AddToDoItemOptions(
                        x.Name,
                        x.Description,
                        ToDoItemType.Reference,
                        false,
                        false,
                        DateTime.Now.ToDateOnly(),
                        TypeOfPeriodicity.Annually,
                        Array.Empty<DayOfYear>(),
                        Array.Empty<byte>(),
                        Array.Empty<DayOfWeek>(),
                        1,
                        0,
                        0,
                        0,
                        ToDoItemChildrenType.RequireCompletion,
                        true,
                        x.DescriptionType,
                        x.Icon,
                        x.Color.ToString(),
                        new(x.Id),
                        new(),
                        x.Link.ToOptionUri(),
                        0
                    ).ToResult()
                )
               .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct)
               .ToResultOnlyAsync();
        }

        return ResultItems.ToResult()
           .IfSuccessForEach(
                x => new AddToDoItemOptions(
                    x.Name,
                    x.Description,
                    ToDoItemType.Reference,
                    false,
                    false,
                    DateTime.Now.ToDateOnly(),
                    TypeOfPeriodicity.Annually,
                    Array.Empty<DayOfYear>(),
                    Array.Empty<byte>(),
                    Array.Empty<DayOfWeek>(),
                    1,
                    0,
                    0,
                    0,
                    ToDoItemChildrenType.RequireCompletion,
                    true,
                    x.DescriptionType,
                    x.Icon,
                    x.Color.ToString(),
                    new(x.Id),
                    new(ToDoItemSelectorViewModel.SelectedItem.Id),
                    x.Link.ToOptionUri(),
                    0
                ).ToResult()
            )
           .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct)
           .ToResultOnlyAsync();
    }

    public Result UpdateItemUi()
    {
        return ToDoItemSelectorViewModel.GetSelectedItem()
           .IfSuccess(
                selectedItem =>
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
                }
            );
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