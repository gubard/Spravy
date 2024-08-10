namespace Spravy.Ui.Extensions;

public static class AvaloniaListExtension
{
    public static Result UpdateUi(
        this AvaloniaList<ToDoItemEntityNotify> list,
        ReadOnlyMemory<ToDoItemEntityNotify> items
    )
    {
        list.RemoveAll(list.Where(x => !items.Span.Contains(x)));
        list.AddRange(items.Where(x => !list.Contains(x)).ToArray());

        return Result.Success;
    }

    public static Result UpdateUi(
        this AvaloniaList<ToDoItemEntityNotify> list,
        AvaloniaList<ToDoItemEntityNotify> items
    )
    {
        list.RemoveAll(list.Where(x => !items.Contains(x)));
        list.AddRange(items.Where(x => !list.Contains(x)).ToArray());

        return Result.Success;
    }

    public static void UpdateUi(
        this AvaloniaList<ToDoItemEntityNotify> list,
        IEnumerable<ToDoItemEntityNotify> items
    )
    {
        list.RemoveAll(list.Where(x => !items.Contains(x)));
        list.AddRange(items.Where(x => !list.Contains(x)).ToArray());
    }

    public static void UpdateUi<T>(this AvaloniaList<T> list, ReadOnlyMemory<T> items)
    {
        list.Clear();
        list.AddRange(items.ToArray());
    }

    public static bool IsEmpty<T>(this AvaloniaList<T> list)
    {
        return list.Count == 0;
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this AvaloniaList<T> list)
    {
        return new(list.ToArray());
    }

    private static int BinarySearch(
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
            return list.BinarySearch(x, mid + 1, high);
        }

        return list.BinarySearch(x, low, mid - 1);
    }

    public static void BinarySort(this AvaloniaList<ToDoItemEntityNotify> list)
    {
        for (var i = 1; i < list.Count; ++i)
        {
            var j = i - 1;
            var key = list[i];
            var pos = list.BinarySearch(key, 0, j);

            while (j >= pos)
            {
                list[j + 1] = list[j];
                j--;
            }

            list[j + 1] = key;
        }
    }
}
