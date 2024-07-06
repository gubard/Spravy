namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsViewModel : ViewModelBase
{
    public ToDoItemsViewModel(IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        Items = new();

        SwitchAllSelectionCommand = SpravyCommand.Create(
            _ =>
                this.PostUiBackground(() =>
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
                    })
                    .ToValueTaskResult()
                    .ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand SwitchAllSelectionCommand { get; }
    public AvaloniaList<ToDoItemEntityNotify> Items { get; }

    [Reactive]
    public bool IsMulti { get; set; }

    [Reactive]
    public TextLocalization? Header { get; set; }

    [Reactive]
    public bool IsExpanded { get; set; } = true;

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> items)
    {
        Items.RemoveAll(Items.Where(x => !items.Span.Contains(x)));

        foreach (var item in items.Span)
        {
            UpdateItemUi(item);
        }

        return Result.Success;
    }

    private int BinarySearch(ToDoItemEntityNotify x, int low, int high)
    {
        if (high <= low)
        {
            return x.OrderIndex > Items[low].OrderIndex ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.OrderIndex == Items[mid].OrderIndex)
        {
            return mid + 1;
        }

        if (x.OrderIndex > Items[mid].OrderIndex)
        {
            return BinarySearch(x, mid + 1, high);
        }

        return BinarySearch(x, low, mid - 1);
    }

    private void BinarySort()
    {
        for (var i = 1; i < Items.Count; ++i)
        {
            var j = i - 1;
            var key = Items[i];
            var pos = BinarySearch(key, 0, j);

            while (j >= pos)
            {
                Items[j + 1] = Items[j];
                j--;
            }

            Items[j + 1] = key;
        }
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        var indexOf = IndexOf(item);

        if (indexOf == -1)
        {
            Items.Add(item);
        }

        BinarySort();

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
