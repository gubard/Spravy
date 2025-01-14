namespace Spravy.Ui.Extensions;

public static class ToDoItemEntityNotifyExtension
{

    private static int BinarySearchOrderIndex(
        this AvaloniaList<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.OrderIndex > list[low].OrderIndex ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.OrderIndex == list[mid].OrderIndex)
        {
            return mid + 1;
        }

        if (x.OrderIndex > list[mid].OrderIndex)
        {
            return list.BinarySearchOrderIndex(x, mid + 1, high);
        }

        return list.BinarySearchOrderIndex(x, low, mid - 1);
    }

    public static Result BinarySortByOrderIndex(this AvaloniaList<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Count; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchOrderIndex(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].OrderIndex != list[j].OrderIndex)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].OrderIndex != key.OrderIndex)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    public static Result BinarySortByName(this AvaloniaList<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Count; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchName(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].Name != list[j].Name)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].Name != key.Name)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    private static int BinarySearchName(
        this AvaloniaList<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return string.Compare(x.Name, list[low].Name, StringComparison.InvariantCultureIgnoreCase) > 0
                ? low + 1
                : low;
        }

        var mid = (low + high) / 2;

        if (x.Name == list[mid].Name)
        {
            return mid + 1;
        }

        if (string.Compare(x.Name, list[mid].Name, StringComparison.InvariantCultureIgnoreCase) > 0)
        {
            return list.BinarySearchName(x, mid + 1, high);
        }

        return list.BinarySearchName(x, low, mid - 1);
    }

    public static Result BinarySortByDueDate(this AvaloniaList<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Count; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchDueDate(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].DueDate != list[j].DueDate)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].DueDate != key.DueDate)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    private static int BinarySearchDueDate(
        this AvaloniaList<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.DueDate > list[low].DueDate ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.DueDate == list[mid].DueDate)
        {
            return mid + 1;
        }

        if (x.DueDate > list[mid].DueDate)
        {
            return list.BinarySearchDueDate(x, mid + 1, high);
        }

        return list.BinarySearchDueDate(x, low, mid - 1);
    }

    private static int BinarySearchLoadedIndex(
        this AvaloniaList<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.LoadedIndex > list[low].LoadedIndex ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.LoadedIndex == list[mid].LoadedIndex)
        {
            return mid + 1;
        }

        if (x.LoadedIndex > list[mid].LoadedIndex)
        {
            return list.BinarySearchLoadedIndex(x, mid + 1, high);
        }

        return list.BinarySearchLoadedIndex(x, low, mid - 1);
    }

    public static Result BinarySortByLoadedIndex(this AvaloniaList<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Count; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchLoadedIndex(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].LoadedIndex != list[j].LoadedIndex)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].LoadedIndex != key.LoadedIndex)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    private static int BinarySearchOrderIndex(
        this Span<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.OrderIndex > list[low].OrderIndex ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.OrderIndex == list[mid].OrderIndex)
        {
            return mid + 1;
        }

        if (x.OrderIndex > list[mid].OrderIndex)
        {
            return list.BinarySearchOrderIndex(x, mid + 1, high);
        }

        return list.BinarySearchOrderIndex(x, low, mid - 1);
    }

    public static Result BinarySortByOrderIndex(this Span<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Length; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchOrderIndex(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].OrderIndex != list[j].OrderIndex)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].OrderIndex != key.OrderIndex)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    public static Result BinarySortByName(this Span<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Length; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchName(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].Name != list[j].Name)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].Name != key.Name)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    private static int BinarySearchName(
        this Span<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return string.Compare(x.Name, list[low].Name, StringComparison.InvariantCultureIgnoreCase) > 0
                ? low + 1
                : low;
        }

        var mid = (low + high) / 2;

        if (x.Name == list[mid].Name)
        {
            return mid + 1;
        }

        if (string.Compare(x.Name, list[mid].Name, StringComparison.InvariantCultureIgnoreCase) > 0)
        {
            return list.BinarySearchName(x, mid + 1, high);
        }

        return list.BinarySearchName(x, low, mid - 1);
    }

    public static Result BinarySortByDueDate(this Span<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Length; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchDueDate(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].DueDate != list[j].DueDate)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].DueDate != key.DueDate)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }

    private static int BinarySearchDueDate(
        this Span<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.DueDate > list[low].DueDate ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.DueDate == list[mid].DueDate)
        {
            return mid + 1;
        }

        if (x.DueDate > list[mid].DueDate)
        {
            return list.BinarySearchDueDate(x, mid + 1, high);
        }

        return list.BinarySearchDueDate(x, low, mid - 1);
    }

    private static int BinarySearchLoadedIndex(
        this Span<ToDoItemEntityNotify> list,
        ToDoItemEntityNotify x,
        int low,
        int high
    )
    {
        if (high <= low)
        {
            return x.LoadedIndex > list[low].LoadedIndex ? low + 1 : low;
        }

        var mid = (low + high) / 2;

        if (x.LoadedIndex == list[mid].LoadedIndex)
        {
            return mid + 1;
        }

        if (x.LoadedIndex > list[mid].LoadedIndex)
        {
            return list.BinarySearchLoadedIndex(x, mid + 1, high);
        }

        return list.BinarySearchLoadedIndex(x, low, mid - 1);
    }

    public static Result BinarySortByLoadedIndex(this Span<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Length; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearchLoadedIndex(key, 0, j);

            while (j >= pos)
            {
                if (list[j + 1].LoadedIndex != list[j].LoadedIndex)
                {
                    list[j + 1] = list[j];
                }

                j--;
            }

            if (list[j + 1].LoadedIndex != key.LoadedIndex)
            {
                list[j + 1] = key;
            }
        }

        return Result.Success;
    }
}