namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class SearchToDoItemsViewModel : NavigatableViewModelBase, IToDoItemEditId, IRemove
{
    private readonly IObjectStorage objectStorage;
    private readonly TaskWork refreshWork;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private string searchText = string.Empty;

    public SearchToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache
    ) : base(true)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.objectStorage = objectStorage;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        SearchTexts = new();
        Commands = new();
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public AvaloniaList<string> SearchTexts { get; }

    public override string ViewId => TypeCache<SearchToDoItemsViewModel>.Name;

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
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.GetRequest(GetToDo.WithDefaultItems.SetSearchText(SearchText), ct)
           .IfSuccessAsync(
                response => response.SearchItems
                   .Items
                   .Select(x => x.Item.Id)
                   .IfSuccessForEach(x => toDoCache.GetToDoItem(x))
                   .IfSuccess(
                        items => this.PostUiBackground(() => ToDoSubItemsViewModel.SetItemsUi(items), ct)
                    ),
                ct
            );
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return ToDoSubItemsViewModel.Stop();
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(
                s => this.PostUiBackground(
                    () =>
                    {
                        SearchText = s.SearchText;
                        SearchTexts.UpdateUi(s.SearchTexts);

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this), ct);
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