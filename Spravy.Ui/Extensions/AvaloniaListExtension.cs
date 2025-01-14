namespace Spravy.Ui.Extensions;

public static class AvaloniaListExtension
{
    public static Result UpdateUi<T>(this AvaloniaList<T> list, List<T> items)
    {
        list.Clear();
        list.AddRange(items);

        return Result.Success;
    }

    public static Result UpdateUi<T>(this AvaloniaList<T> list, AvaloniaList<T> items)
    {
        list.Clear();
        list.AddRange(items);

        return Result.Success;
    }

    public static Result UpdateUi<T>(this AvaloniaList<T> list, IEnumerable<T> items)
    {
        list.Clear();
        list.AddRange(items);

        return Result.Success;
    }

    public static Result UpdateUi<T>(this AvaloniaList<T> list, ReadOnlyMemory<T> items)
    {
        list.Clear();
        list.AddRange(items.ToArray());

        return Result.Success;
    }

    public static Result UpdateUi<T>(this AvaloniaList<T> list, T[] items)
    {
        list.Clear();
        list.AddRange(items);

        return Result.Success;
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