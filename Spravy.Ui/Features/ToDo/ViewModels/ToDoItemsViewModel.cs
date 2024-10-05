using Spravy.Ui.Features.ToDo.Errors;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase
{
    private readonly SortBy sortBy;

    [ObservableProperty]
    private bool isExpanded = true;

    public ToDoItemsViewModel(
        SortBy sortBy,
        TextLocalization header,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Header = header;
        this.sortBy = sortBy;

        SwitchAllSelectionCommand = SpravyCommand.Create(
            ct =>
                this.PostUiBackground(
                        () =>
                        {
                            if (Items.All(x => x.IsSelected))
                            {
                                foreach (var item in Items)
                                {
                                    item.IsSelected = false;
                                }
                            }
                            else
                            {
                                foreach (var item in Items)
                                {
                                    item.IsSelected = true;
                                }
                            }

                            return Result.Success;
                        },
                        ct
                    )
                    .ToValueTaskResult()
                    .ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );
    }

    public TextLocalization Header { get; }
    public SpravyCommand SwitchAllSelectionCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; } = new();

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        var removeItems = Items.Where(x => !items.Span.Contains(x)).ToArray();
        Items.RemoveAll(removeItems);

        foreach (var item in items.Span)
        {
            AddOrUpdateUi(item);
        }

        return Result.Success;
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
        }

        switch (sortBy)
        {
            case SortBy.OrderIndex:
                Items.BinarySortByOrderIndex();
                break;
            case SortBy.LoadedIndex:
                Items.BinarySortByLoadedIndex();
                break;
            default:
                return new(new SortByOutOfRangeError(sortBy));
        }

        return Result.Success;
    }

    public Result RemoveUi(ToDoItemEntityNotify item)
    {
        Items.Remove(item);

        return Result.Success;
    }

    public Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
