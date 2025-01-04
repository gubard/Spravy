namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ChangeToDoItemOrderIndexViewModel : ToDoItemEditIdViewModel, IToDoItemsView, IApplySettings
{
    private readonly IToDoService toDoService;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private bool isAfter = true;

    [ObservableProperty]
    private ToDoItemEntityNotify? selectedItem;

    public ChangeToDoItemOrderIndexViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IToDoService toDoService,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache
    ) : base(editItem, editItems)
    {
        this.toDoService = toDoService;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
    }

    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    public override string ViewId => TypeCache<DialogableViewModelBase>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return SelectedItem.IfNotNull(nameof(SelectedItem))
           .IfSuccessAsync(
                si => ResultIds.ToResult()
                   .IfSuccessForEach(x => new UpdateOrderIndexToDoItemOptions(x, si.Id, IsAfter).ToResult())
                   .IfSuccessAsync(options => toDoService.UpdateToDoItemOrderIndexAsync(options, ct), ct),
                ct
            );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        var parent = ResultItems.Span[0].Parent;

        return Result.Success
           .IfSuccess(
                () => parent is not null ? parent.Children.ToArray().ToReadOnlyMemory().ToResult()
                    : toDoCache.GetRootItems()
            )
           .IfSuccess(
                items => this.PostUiBackground(
                    () => SetItemsUi(items.OrderBy(x => x.OrderIndex))
                       .IfSuccess(
                            () => items.IfSuccessForEach(
                                x =>
                                {
                                    x.IsIgnore = false;

                                    return Result.Success;
                                }
                            )
                        )
                       .IfSuccess(
                            () => ResultItems.IfSuccessForEach(
                                x =>
                                {
                                    x.IsIgnore = true;

                                    return Result.Success;
                                }
                            )
                        ),
                    ct
                )
            )
           .IfSuccessAsync(
                () => toDoUiService.GetRequest(
                    parent is not null
                        ? GetToDo.WithDefaultItems.SetChildrenItem(parent.Id)
                        : GetToDo.WithDefaultItems.SetIsRootItems(true),
                    ct
                ),
                ct
            )
           .IfSuccessAsync(
                response =>
                    parent is not null
                        ? response.ChildrenItems
                           .Select(x => x.Children)
                           .SelectMany()
                           .Select(x => x.Item.Id)
                           .IfSuccessForEach(toDoCache.GetToDoItem)
                        : response.RootItems
                           .Items
                           .Select(x => x.Item.Id)
                           .IfSuccessForEach(toDoCache.GetToDoItem),
                ct
            )
           .IfSuccessAsync(
                items => this.PostUiBackground(
                    () => SetItemsUi(items.OrderBy(x => x.OrderIndex)),
                    ct
                ),
                ct
            );
    }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> newItems)
    {
        return Items.UpdateUi(newItems).ToResultOnly();
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> _)
    {
        return new(new NotImplementedError(nameof(AddOrUpdateUi)));
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Items.RemoveAll(items.ToArray());

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
}