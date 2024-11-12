using Spravy.Ui.Features.ToDo.Errors;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase
{
    private readonly SortBy sortBy;

    [ObservableProperty]
    private bool isExpanded = true;
    private readonly AvaloniaList<ToDoItemEntityNotify> toDoItems = new();

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
                            if (ToDoItems.All(x => x.IsSelected))
                            {
                                foreach (var item in ToDoItems)
                                {
                                    item.IsSelected = false;
                                }
                            }
                            else
                            {
                                foreach (var item in ToDoItems)
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

    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> ToDoItems
    {
        get => toDoItems;
    }

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        var removeItems = ToDoItems.Where(x => !items.Span.Contains(x)).ToArray();
        toDoItems.RemoveAll(removeItems);
        AddOrUpdateUi(items);

        return Result.Success;
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        var notContains = items.Where(x => !ToDoItems.Contains(x));
        toDoItems.AddRange(notContains.ToArray());

        switch (sortBy)
        {
            case SortBy.OrderIndex:
                toDoItems.BinarySortByOrderIndex();
                break;
            case SortBy.LoadedIndex:
                toDoItems.BinarySortByLoadedIndex();
                break;
            default:
                return new(new SortByOutOfRangeError(sortBy));
        }

        return Result.Success;
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        toDoItems.RemoveAll(items.ToArray());

        return Result.Success;
    }
}
