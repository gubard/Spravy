namespace Spravy.Domain.Extensions;

public static class MemoryExtension
{
    public static Span<char> Join(this ReadOnlyMemory<string> memory, ReadOnlySpan<char> separator)
    {
        var result = new Span<char>(
            new char[
                memory.Select(x => x.Length).Sum()
                    + memory.Length * separator.Length
                    - separator.Length
            ]
        );

        var currentIndex = 0;

        for (var index = 0; index < memory.Length; index++)
        {
            var chars = memory.Span[index].AsSpan();
            chars.CopyTo(result.Slice(currentIndex, chars.Length));
            currentIndex += chars.Length;

            if (memory.Length - 1 != index)
            {
                separator.CopyTo(result.Slice(currentIndex, separator.Length));
                currentIndex += separator.Length;
            }
        }

        return result;
    }

    public static int Sum(this ReadOnlyMemory<int> memory)
    {
        var result = 0;

        for (var index = 0; index < memory.Span.Length; index++)
        {
            result += memory.Span[index];
        }

        return result;
    }

    public static int Sum(this Span<int> span)
    {
        var result = 0;

        for (var index = 0; index < span.Length; index++)
        {
            result += span[index];
        }

        return result;
    }

    public static Span<TSource> SelectMany<TSource>(
        this ReadOnlyMemory<ReadOnlyMemory<TSource>> memory
    )
    {
        var result = new Span<TSource>(new TSource[memory.Select(x => x.Length).Sum()]);
        var currentIndex = 0;

        for (var index = 0; index < memory.Length; index++)
        {
            if (memory.Span[index].IsEmpty)
            {
                continue;
            }

            memory.Span[index].Span.CopyTo(result.Slice(currentIndex, memory.Span[index].Length));
            currentIndex += memory.Length;
        }

        return result;
    }

    public static bool All<TSource>(
        this ReadOnlyMemory<TSource> source,
        Func<TSource, bool> predicate
    )
    {
        foreach (var value in source.Span)
        {
            if (!predicate.Invoke(value))
            {
                return false;
            }
        }

        return true;
    }

    public static TSource? FirstOrDefault<TSource>(
        this ReadOnlyMemory<TSource> source,
        Func<TSource, bool> predicate
    )
    {
        foreach (var value in source.Span)
        {
            if (predicate.Invoke(value))
            {
                return value;
            }
        }

        return default;
    }

    public static ReadOnlyMemory<TSource> OrderBy<TSource>(
        this ReadOnlyMemory<TSource> source,
        Func<TSource, uint> keySelector
    )
    {
        var result = new Memory<TSource>(new TSource[source.Length]);
        source.CopyTo(result);
        BinarySort(result.Span, keySelector);

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

    private static void BinarySort<TSource>(Span<TSource> a, Func<TSource, uint> keySelector)
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

    public static ReadOnlyMemory<TResult> Select<TSource, TResult>(
        this ReadOnlyMemory<TSource> memory,
        Func<TSource, TResult> selector
    )
    {
        if (memory.IsEmpty)
        {
            return ReadOnlyMemory<TResult>.Empty;
        }

        var result = new TResult[memory.Length];

        for (var index = 0; index < memory.Length; index++)
        {
            result[index] = selector.Invoke(memory.Span[index]);
        }

        return result;
    }

    public static ReadOnlyMemory<TSource> Where<TSource>(
        this ReadOnlyMemory<TSource> memory,
        Func<TSource, bool> predicate
    )
    {
        if (memory.IsEmpty)
        {
            return ReadOnlyMemory<TSource>.Empty;
        }

        var result = new Memory<TSource>(new TSource[memory.Length]);
        var resultIndex = 0;

        for (var index = 0; index < memory.Length; index++)
        {
            if (!predicate.Invoke(memory.Span[index]))
            {
                continue;
            }

            result.Span[resultIndex] = memory.Span[index];
            resultIndex++;
        }

        if (resultIndex == 0)
        {
            return ReadOnlyMemory<TSource>.Empty;
        }

        return result.Slice(0, resultIndex);
    }

    public static Result<T> GetSingle<T>(this ReadOnlyMemory<T> memory, string arrayName)
        where T : notnull
    {
        if (memory.IsEmpty)
        {
            return new(new EmptyArrayError(arrayName));
        }

        if (memory.Length == 1)
        {
            return new(memory.Span[0]);
        }

        return new(new MultiValuesArrayError(arrayName, (ulong)memory.Length));
    }

    public static T GetSingle<T>(this Memory<T> memory)
    {
        if (memory.Length == 1)
        {
            return memory.Span[0];
        }

        throw new(memory.Length.ToString());
    }
}
