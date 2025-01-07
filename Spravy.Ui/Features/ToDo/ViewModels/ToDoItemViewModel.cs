namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemViewModel : NavigatableViewModelBase, IRemove, IToDoItemEditId
{
    private readonly IObjectStorage objectStorage;
    private readonly TaskWork refreshWork;
    private readonly IToDoUiService toDoUiService;
    private readonly IToDoCache toDoCache;

    [ObservableProperty]
    private bool isMulti;

    public ToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IToDoUiService toDoUiService,
        IToDoCache toDoCache
    ) : base(true)
    {
        this.objectStorage = objectStorage;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoUiService = toDoUiService;
        this.toDoCache = toDoCache;
        Item = item;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        Commands = new(Item.Commands);
        ToDoSubItemsViewModel.SetItemsUi(Item.Children.ToArray()).ThrowIfError();
    }

    public ToDoItemEntityNotify Item { get; }
    public ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }

    public override string ViewId => $"{TypeCache<ToDoItemViewModel>.Name}:{Item.Id}";

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        return ToDoSubItemsViewModel.RemoveUi(items);
    }

    public Result<ToDoItemEditId> GetToDoItemEditId()
    {
        if (!ToDoSubItemsViewModel.List.IsMulti)
        {
            return new ToDoItemEditId(Item.ToOption(), ReadOnlyMemory<ToDoItemEntityNotify>.Empty).ToResult();
        }

        return ToDoSubItemsViewModel.GetSelectedItems()
           .IfSuccess(selected => new ToDoItemEditId(Item.ToOption(), selected).ToResult());
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return RefreshCore().ConfigureAwait(false);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(IsMulti))
        {
            UpdateCommandsUi();
            ToDoSubItemsViewModel.List.IsMulti = IsMulti;
        }
    }

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.GetRequest(
                GetToDo.WithDefaultItems
                   .SetParentItem(Item.Id)
                   .SetChildrenItem(Item.Id)
                   .SetItem(Item.Id),
                ct
            )
           .IfSuccessAsync(
                _ => this.PostUiBackground(() => ToDoSubItemsViewModel.SetItemsUi(Item.Children.ToArray()), ct),
                ct
            )
           .IfSuccessAsync(
                () => this.PostUiBackground(
                    () =>
                    {
                        UpdateCommandsUi();

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return ToDoSubItemsViewModel.Stop();
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new ToDoItemViewModelSetting(this), ct);
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<ToDoItemViewModelSetting>(ViewId, ct)
           .IfSuccessAsync(
                s => this.PostUiBackground(
                    () =>
                    {
                        ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                        IsMulti = s.IsMulti;

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    private void UpdateCommandsUi()
    {
        if (IsMulti)
        {
            Commands.UpdateUi(UiHelper.ToDoItemCommands);
        }
        else
        {
            Commands.UpdateUi(Item.Commands);
        }
    }
}