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

    public static ReadOnlyMemory<TResult> Select<TSource, TResult>(
        this ReadOnlyMemory<TSource> memory,
        Func<TSource, TResult> selector
    )
    {
        var result = new TResult[memory.Length];

        for (var index = 0; index < memory.Length; index++)
        {
            result[index] = selector.Invoke(memory.Span[index]);
        }

        return result;
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
