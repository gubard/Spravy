namespace Spravy.Ui.Extensions;

public static class AvaloniaListExtension
{
    public static void Update<T>(this AvaloniaList<T> list, ReadOnlyMemory<T> items)
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
}
