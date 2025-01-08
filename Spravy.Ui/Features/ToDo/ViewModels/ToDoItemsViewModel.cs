using Spravy.Ui.Features.ToDo.Errors;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase
{
    private readonly AvaloniaList<ToDoItemEntityNotify> toDoItems = new();
    private readonly ViewModelSortBy viewModelSortBy;

    [ObservableProperty]
    private bool isExpanded = true;

    [ObservableProperty]
    private SortByToDoItem sortBy;

    public ToDoItemsViewModel(
        ViewModelSortBy viewModelSortBy,
        TextLocalization header,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Header = header;
        this.viewModelSortBy = viewModelSortBy;

        SwitchAllSelectionCommand = SpravyCommand.Create(
            ct => this.PostUiBackground(
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
    public IAvaloniaReadOnlyList<ToDoItemEntityNotify> ToDoItems => toDoItems;

    public Result SetItemsUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        var removeItems = ToDoItems.Where(x => !items.Span.Contains(x) || !x.IsUpdated).ToArray();
        toDoItems.RemoveAll(removeItems);
        AddOrUpdateUi(items);

        return Result.Success;
    }

    public Result AddOrUpdateUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        var notContains = items.Where(x => x.IsUpdated && !ToDoItems.Contains(x));
        toDoItems.AddRange(notContains.ToArray());

        return Sort();
    }

    public Result RemoveUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        toDoItems.RemoveAll(items.ToArray());

        return Result.Success;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(SortBy))
        {
            Sort();
        }
    }

    private Result Sort()
    {
        return SortBy switch
        {
            SortByToDoItem.Index => viewModelSortBy switch
            {
                ViewModelSortBy.OrderIndex => toDoItems.BinarySortByOrderIndex(),
                ViewModelSortBy.LoadedIndex => toDoItems.BinarySortByLoadedIndex(),
                _ => new(new ViewModelSortByOutOfRangeError(viewModelSortBy)),
            },
            SortByToDoItem.Name => toDoItems.BinarySortByName(),
            SortByToDoItem.DueDate => toDoItems.BinarySortByName(),
            _ => new(new SortByToDoItemOutOfRangeError(SortBy)),
        };
    }
}