namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class SearchToDoItemsViewModel : NavigatableViewModelBase, IToDoItemsView
{
    private readonly TaskWork refreshWork;
    private readonly IToDoUiService toDoUiService;
    private readonly IObjectStorage objectStorage;
    private readonly SpravyCommandNotifyService spravyCommandNotifyService;

    [ObservableProperty]
    private string searchText = string.Empty;

    public SearchToDoItemsViewModel(
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        SpravyCommandNotifyService spravyCommandNotifyService,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoUiService toDoUiService
    )
        : base(true)
    {
        this.spravyCommandNotifyService = spravyCommandNotifyService;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.objectStorage = objectStorage;
        this.toDoUiService = toDoUiService;
        Commands = new();
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId
    {
        get => TypeCache<SearchToDoItemsViewModel>.Type.Name;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false);
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateSearchToDoItemsAsync(SearchText, ToDoSubItemsViewModel, ct);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId, ct)
            .IfSuccessAsync(
                s =>
                    this.PostUiBackground(
                        () =>
                        {
                            SearchText = s.SearchText;

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
                Commands.UpdateUi(spravyCommandNotifyService.SearchToDoItemsMulti);
            }
            else
            {
                Commands.Clear();
            }
        }
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.ClearExceptUi(items);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return ToDoSubItemsViewModel.AddOrUpdateUi(item);
    }
}
