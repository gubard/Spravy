namespace Spravy.Ui.Features.ToDo.ViewModels;

public class TodayToDoItemsViewModel : NavigatableViewModelBase, IToDoItemEditId, IRemove
{
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;

    public TodayToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache
    ) : base(true)
    {
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId => TypeCache<TodayToDoItemsViewModel>.Name;

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.RemoveUi(items);
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new(new NonItemSelectedError());
        }

        return ToDoSubItemsViewModel.GetSelectedItems()
           .IfSuccess(selected => new ToDoItemEditId(new(), selected).ToResult());
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return toDoUiService.GetRequest(GetToDo.WithDefaultItems.SetIsTodayItems(true), ct)
           .IfSuccessAsync(
                response => response.TodayItems
                   .Items
                   .Select(x => x.Item.Id)
                   .IfSuccessForEach(x => toDoCache.GetToDoItem(x))
                   .IfSuccessAsync(
                        items => this.PostUiBackground(() => ToDoSubItemsViewModel.SetItemsUi(items), ct)
                           .IfSuccessAsync(
                                () => toDoUiService.GetRequest(
                                    GetToDo.Default.SetParentItems(items.Select(x => x.Id)),
                                    ct
                                ),
                                ct
                            ),
                        ct
                    ),
                ct
            )
           .ToResultOnlyAsync();
    }

    public override Result Stop()
    {
        return ToDoSubItemsViewModel.Stop();
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                Commands.UpdateUi(spravyCommandNotifyService.ToDoItemCommands);
            }
            else
            {
                Commands.Clear();
            }
        }
    }
}