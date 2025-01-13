namespace Spravy.Ui.Features.ToDo.ViewModels;

public class LeafToDoItemsViewModel : NavigatableViewModelBase, IObjectParameters, IToDoItemEditId
{
    private static readonly ReadOnlyMemory<char> headerParameterName = nameof(Header).AsMemory();
    private readonly IObjectStorage objectStorage;

    private readonly TaskWork refreshWork;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;

    public LeafToDoItemsViewModel(
        Option<ToDoItemEntityNotify> item,
        ReadOnlyMemory<ToDoItemEntityNotify> items,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IObjectStorage objectStorage,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache
    ) : base(true)
    {
        Item = item;
        Items = items;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        Commands = new();
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.objectStorage = objectStorage;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        ToDoSubItemsViewModel.List.PropertyChanged += OnPropertyChanged;
    }

    public Option<ToDoItemEntityNotify> Item { get; }
    public ReadOnlyMemory<ToDoItemEntityNotify> Items { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public string Header
    {
        get
        {
            if (!Item.TryGetValue(out var item))
            {
                return Items.Select(x => x.Name).JoinString(",");
            }

            if (Items.IsEmpty)
            {
                return item.Name;
            }

            return Items.Select(x => x.Name).JoinString(",");
        }
    }

    public override string ViewId => TypeCache<LeafToDoItemsViewModel>.Name;

    public Result<string> GetParameter(ReadOnlySpan<char> parameterName)
    {
        if (headerParameterName.Span.AreEquals(parameterName))
        {
            return Header.ToResult();
        }

        return new(new NotFoundNamedError(parameterName.ToString()));
    }

    public Result SetParameter(ReadOnlySpan<char> parameterName, ReadOnlySpan<char> parameterValue)
    {
        return new(new NotImplementedError(nameof(SetParameter)));
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new(new NonItemSelectedError());
        }

        return ToDoSubItemsViewModel.GetSelectedItems()
           .IfSuccess(selected => new ToDoItemEditId(Item, selected).ToResult());
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshCore(CancellationToken ct)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.GetRequest(GetToDo.WithDefaultItems.SetLeafItems(GetIds()), ct)
           .IfSuccessAsync(
                response => response.LeafItems
                   .Select(x => x.Leafs)
                   .SelectMany()
                   .Select(x => x.Item.Id)
                   .IfSuccessForEach(x => toDoCache.GetToDoItem(x)),
                ct
            )
           .IfSuccessAsync(
                items => this.PostUiBackground(() => ToDoSubItemsViewModel.SetItemsUi(items), ct)
                   .IfSuccessAsync(
                        () => toDoUiService.GetRequest(GetToDo.Default.SetParentItems(items.Select(x => x.Id)), ct),
                        ct
                    ),
                ct
            )
           .ToResultOnlyAsync();
    }

    private ReadOnlyMemory<Guid> GetIds()
    {
        if (Items.IsEmpty)
        {
            return new[]
            {
                Item.GetValue().ThrowIfError().Id,
            };
        }

        return Items.Select(x => x.Id);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return ToDoSubItemsViewModel.Stop();
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(
                setting => this.PostUiBackground(
                    () =>
                    {
                        ToDoSubItemsViewModel.List.IsMulti = setting.IsMulti;
                        ToDoSubItemsViewModel.List.GroupBy = setting.GroupBy;

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LeafToDoItemsViewModelSetting(this), ct);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ToDoSubItemsViewModel.List.IsMulti))
        {
            if (ToDoSubItemsViewModel.List.IsMulti)
            {
                Commands.UpdateUi(UiHelper.ToDoItemCommands);
            }
            else
            {
                Commands.Clear();
            }
        }
    }
}