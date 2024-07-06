namespace Spravy.Domain.Extensions;

public static class EnumerableExtension
{
    public static object? ElementAt(this IEnumerable items, int index)
    {
        if (items is IList list)
        {
            return list[index];
        }

        return Enumerable.ElementAt(items.Cast<object>(), index);
    }

    public static int Count(this IEnumerable items)
    {
        if (TryGetCountFast(items, out var count))
        {
            return count;
        }

        return 0;
    }

    public static bool TryGetCountFast(this IEnumerable? items, out int count)
    {
        if (items is not null)
        {
            if (items is ICollection collection)
            {
                count = collection.Count;

                return true;
            }

            if (items is IReadOnlyCollection<object> readOnly)
            {
                count = readOnly.Count;

                return true;
            }
        }

        count = 0;

        return false;
    }

    public static bool IsEmpty<TItem>(this IEnumerable<TItem> enumerable)
    {
        return !enumerable.Any();
    }

    public static IEnumerable<TItem> ThrowIfNullOrEmpty<TItem>(
        this IEnumerable<TItem>? enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string paramName = ""
    )
    {
        var array = enumerable.ThrowIfNull().ToArray();
        array.ThrowIfNull(paramName);
        array.ThrowIfEmpty(paramName);

        return array;
    }

    public static TItem[] ThrowIfNullOrEmpty<TItem>(
        this TItem[]? enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string paramName = ""
    )
    {
        var array = enumerable.ThrowIfNull().ToArray();
        array.ThrowIfNull(paramName);
        array.ThrowIfEmpty(paramName);

        return array;
    }

    public static IEnumerable<TItem> ThrowIfEmpty<TItem>(
        this IEnumerable<TItem> enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string paramName = ""
    )
    {
        var array = enumerable.ToArray();

        return array.IsEmpty() ? throw new EmptyEnumerableException(paramName) : array;
    }

    public static string JoinString<TEnumerable>(this TEnumerable enumerable, string separator)
        where TEnumerable : IEnumerable
    {
        return string.Join(separator, enumerable.OfType<object>());
    }

    public static string JoinString<T>(this ReadOnlyMemory<T> memory, string separator)
    {
        return string.Join(separator, memory.ToArray());
    }

    public static string JoinString<TEnumerable>(this TEnumerable enumerable)
        where TEnumerable : IEnumerable
    {
        return enumerable.JoinString(string.Empty);
    }

    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var result = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            result.Columns.Add(prop.Name, prop.PropertyType);
        }

        foreach (var item in items)
        {
            var values = new object?[props.Length];

            for (var i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null);
            }

            result.Rows.Add(values);
        }

        return result;
    }
}
