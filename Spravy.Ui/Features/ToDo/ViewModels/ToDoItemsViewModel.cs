namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class ToDoItemsViewModel : ViewModelBase
{
    [ObservableProperty]
    private TextLocalization? header;

    [ObservableProperty]
    private bool isExpanded = true;

    public ToDoItemsViewModel(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        Items = new();

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

    public SpravyCommand SwitchAllSelectionCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Items.RemoveAll(Items.Where(x => !items.Span.Contains(x)));

        foreach (var item in items.Span)
        {
            UpdateItemUi(item);
        }

        return Result.Success;
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        var indexOf = IndexOf(item);

        if (indexOf == -1)
        {
            Items.Add(item);
        }

        Items.BinarySort();

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

    public Result RemoveItemUi(ToDoItemEntityNotify item)
    {
        Items.Remove(item);

        return Result.Success;
    }
}
