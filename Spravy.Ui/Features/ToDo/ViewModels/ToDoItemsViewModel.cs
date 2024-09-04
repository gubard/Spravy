using Spravy.Ui.Features.ToDo.Errors;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase, IToDoItemsView
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
        Items = new();
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
    public AvaloniaList<ToDoItemEntityNotify> Items { get; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Items.RemoveAll(Items.Where(x => !items.Span.Contains(x)));

        foreach (var item in items.Span)
        {
            AddOrUpdateUi(item);
        }

        return Result.Success;
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        var indexOf = IndexOf(item);

        if (indexOf == -1)
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

    private int IndexOf(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0)
        {
            return -1;
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Id == Items[i].Id)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetNeedIndex(ToDoItemEntityNotify obj)
    {
        if (Items.Count == 0)
        {
            return 0;
        }

        if (obj.OrderIndex > Items[^1].OrderIndex)
        {
            return Items.Count;
        }

        for (var i = 0; i < Items.Count; i++)
        {
            if (obj.Id == Items[i].Id)
            {
                return i;
            }

            if (obj.OrderIndex == Items[i].OrderIndex)
            {
                return i;
            }

            if (obj.OrderIndex > Items[i].OrderIndex)
            {
                continue;
            }

            return i;
        }

        return Items.Count;
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
