namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemViewModel : NavigatableViewModelBase, IRemove, IToDoItemEditId
{
    private readonly IObjectStorage objectStorage;
    private readonly TaskWork refreshWork;
    private readonly IToDoUiService toDoUiService;

    [ObservableProperty]
    private bool isMulti;

    public ToDoItemViewModel(
        ToDoItemEntityNotify item,
        IObjectStorage objectStorage,
        ToDoSubItemsViewModel toDoSubItemsViewModel,
        IErrorHandler errorHandler,
        IToDoUiService toDoUiService
    ) : base(true)
    {
        this.objectStorage = objectStorage;
        ToDoSubItemsViewModel = toDoSubItemsViewModel;
        this.toDoUiService = toDoUiService;
        Item = item;
        refreshWork = TaskWork.Create(errorHandler, RefreshCoreAsync);
        Commands = new(Item.Commands);

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsMulti))
            {
                UpdateCommands();
                ToDoSubItemsViewModel.List.IsMulti = IsMulti;
            }
        };
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

    private async ValueTask<Result> RefreshCore()
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private Cvtar RefreshCoreAsync(CancellationToken ct)
    {
        return toDoUiService.UpdateItemChildrenAsync(Item, ToDoSubItemsViewModel, ct)
           .IfSuccessAsync(
                () =>
                {
                    UpdateCommands();

                    return Result.Success;
                },
                ct
            );
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
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

    private void UpdateCommands()
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