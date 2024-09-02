namespace Spravy.Domain.Extensions;

public static class SpanExtension
{
    public static bool Contains<TSource>(this Span<TSource> source, TSource item)
    {
        if (source.IsEmpty)
        {
            return false;
        }

        foreach (var element in source)
        {
            if (element is null)
            {
                if (item is null)
                {
                    return true;
                }

                continue;
            }

            if (element.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    public static Span<string> DistinctIgnoreNullOrWhiteSpace(this Span<string?> source)
    {
        var result = new Span<string>(new string[source.Length]);
        var resultIndex = 0;

        foreach (var current in source)
        {
            if (current.IsNullOrWhiteSpace())
            {
                continue;
            }

            if (result.Contains(current))
            {
                continue;
            }

            result[resultIndex] = current;
            resultIndex++;
        }

        return result.Slice(0, resultIndex);
    }

    public static Span<TSource> DistinctIgnoreNull<TSource>(this Span<TSource?> source)
    {
        var result = new Span<TSource>(new TSource[source.Length]);
        var resultIndex = 0;

        foreach (var current in source)
        {
            if (current is null)
            {
                continue;
            }

            if (result.Contains(current))
            {
                continue;
            }

            result[resultIndex] = current;
            resultIndex++;
        }

        return result.Slice(0, resultIndex);
    }

    public static void BinarySortDefault<TSource, TValue>(
        this Span<TSource> a,
        Func<TSource, TValue> keySelector
    )
    {
        for (var i = 1; i < a.Length; ++i)
        {
            var j = i - 1;
            var key = a[i];
            var keyValue = keySelector.Invoke(a[i]);
            var pos = BinarySearchDefault(a, keyValue, 0, j, keySelector);

            while (j >= pos)
            {
                a[j + 1] = a[j];
                j--;
            }

            a[j + 1] = key;
        }
    }

    private static int BinarySearchDefault<TSource, TValue>(
        Span<TSource> a,
        TValue x,
        int low,
        int high,
        Func<TSource, TValue> keySelector
    )
    {
        if (high <= low)
        {
            return Comparer<TValue>.Default.Compare(x, keySelector.Invoke(a[low])) > 0
                ? low + 1
                : low;
        }

        var mid = (low + high) / 2;
        var midValue = keySelector.Invoke(a[mid]);

        if (Comparer<TValue>.Default.Compare(x, midValue) == 0)
        {
            return mid + 1;
        }

        if (Comparer<TValue>.Default.Compare(x, midValue) > 0)
        {
            return BinarySearchDefault(a, x, mid + 1, high, keySelector);
        }

        return BinarySearchDefault(a, x, low, mid - 1, keySelector);
    }

    public static int GetStringsLength(this ReadOnlySpan<string> span)
    {
        var result = 0;

        foreach (var t in span)
        {
            result += t.Length;
        }

        return result;
    }

    public static TSource? FirstOrDefault<TSource>(
        this ReadOnlySpan<TSource> source,
        Func<TSource, bool> predicate
    )
    {
        foreach (var value in source)
        {
            if (predicate.Invoke(value))
            {
                return value;
            }
        }

        return default;
    }

    public static bool All<TSource>(
        this ReadOnlySpan<TSource> source,
        Func<TSource, bool> predicate
    )
    {
        foreach (var value in source)
        {
            if (!predicate.Invoke(value))
            {
                return false;
            }
        }

        return true;
    }

    public static ReadOnlySpan<TResult> Select<TSource, TResult>(
        this ReadOnlySpan<TSource> memory,
        Func<TSource, TResult> selector
    )
    {
        if (memory.IsEmpty)
        {
            return ReadOnlySpan<TResult>.Empty;
        }

        var result = new TResult[memory.Length];

        for (var index = 0; index < memory.Length; index++)
        {
            result[index] = selector.Invoke(memory[index]);
        }

        return result;
    }

    public static Span<TSource> SelectMany<TSource>(
        this ReadOnlySpan<ReadOnlyMemory<TSource>> memory
    )
    {
        var result = new Span<TSource>(new TSource[memory.Select(x => x.Length).Sum()]);
        var currentIndex = 0;

        for (var index = 0; index < memory.Length; index++)
        {
            if (memory[index].IsEmpty)
            {
                continue;
            }

            memory[index].Span.CopyTo(result.Slice(currentIndex, memory[index].Length));
            currentIndex += memory.Length;
        }

        return result;
    }

    public static int Sum(this ReadOnlySpan<int> span)
    {
        var result = 0;

        foreach (var t in span)
        {
            result += t;
        }

        return result;
    }

    public static Span<char> Join(this ReadOnlySpan<string> span, ReadOnlySpan<char> separator)
    {
        var result = new Span<char>(
            new char[span.GetStringsLength() + span.Length * separator.Length - separator.Length]
        );

        var currentIndex = 0;

        for (var index = 0; index < span.Length; index++)
        {
            var chars = span[index].AsSpan();
            chars.CopyTo(result.Slice(currentIndex, chars.Length));
            currentIndex += chars.Length;

            if (span.Length - 1 != index)
            {
                separator.CopyTo(result.Slice(currentIndex, separator.Length));
                currentIndex += separator.Length;
            }
        }

        return result;
    }

    public static ReadOnlySpan<TSource> OrderBy<TSource>(
        this Span<TSource> source,
        Func<TSource, uint> keySelector
    )
    {
        var result = new Span<TSource>(new TSource[source.Length]);
        source.CopyTo(result);
        BinarySort(result, keySelector);

        return result;
    }

    public static ReadOnlySpan<TSource> OrderBy<TSource>(
        this ReadOnlySpan<TSource> source,
        Func<TSource, uint> keySelector
    )
    {
        var result = new Span<TSource>(new TSource[source.Length]);
        source.CopyTo(result);
        BinarySort(result, keySelector);

        return result;
    }

    private static int BinarySearch<TSource>(
        Span<TSource> a,
        uint x,
        int low,
        int high,
        Func<TSource, uint> keySelector
    )
    {
        if (high <= low)
        {
            return x > keySelector.Invoke(a[low]) ? low + 1 : low;
        }

        var mid = (low + high) / 2;
        var midValue = keySelector.Invoke(a[mid]);

        if (x == midValue)
        {
            return mid + 1;
        }

        if (x > midValue)
        {
            return BinarySearch(a, x, mid + 1, high, keySelector);
        }

        return BinarySearch(a, x, low, mid - 1, keySelector);
    }

    private static int BinarySearch<TSource, TValue>(
        Span<TSource> a,
        TValue x,
        int low,
        int high,
        Func<TSource, TValue> keySelector
    )
        where TValue : IComparable<TValue>
    {
        if (high <= low)
        {
            return x.CompareTo(keySelector.Invoke(a[low])) > 0 ? low + 1 : low;
        }

        var mid = (low + high) / 2;
        var midValue = keySelector.Invoke(a[mid]);

        if (x.CompareTo(midValue) == 0)
        {
            return mid + 1;
        }

        if (x.CompareTo(midValue) > 0)
        {
            return BinarySearch(a, x, mid + 1, high, keySelector);
        }

        return BinarySearch(a, x, low, mid - 1, keySelector);
    }

    public static void BinarySort<TSource>(this Span<TSource> a, Func<TSource, uint> keySelector)
    {
        for (var i = 1; i < a.Length; ++i)
        {
            var j = i - 1;
            var key = a[i];
            var keyValue = keySelector.Invoke(a[i]);
            var pos = BinarySearch(a, keyValue, 0, j, keySelector);

            while (j >= pos)
            {
                a[j + 1] = a[j];
                j--;
            }

            a[j + 1] = key;
        }
    }

    public static void BinarySort<TSource, TValue>(
        this Span<TSource> a,
        Func<TSource, TValue> keySelector
    )
        where TValue : IComparable<TValue>
    {
        for (var i = 1; i < a.Length; ++i)
        {
            var j = i - 1;
            var key = a[i];
            var keyValue = keySelector.Invoke(a[i]);
            var pos = BinarySearch(a, keyValue, 0, j, keySelector);

            while (j >= pos)
            {
                a[j + 1] = a[j];
                j--;
            }

            a[j + 1] = key;
        }
    }
}
