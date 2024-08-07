﻿namespace Spravy.Domain.Extensions;

public static class MemoryExtension
{
    public static ReadOnlyMemory<TSource> SelectMany<TSource>(
        this ReadOnlyMemory<ReadOnlyMemory<TSource>> memory
    )
    {
        var result = new Memory<TSource>(new TSource[memory.Span.Select(x => x.Length).Sum()]);
        var currentIndex = 0;

        for (var index = 0; index < memory.Length; index++)
        {
            if (memory.Span[index].IsEmpty)
            {
                continue;
            }

            memory.Span[index].CopyTo(result.Slice(currentIndex, memory.Span[index].Length));
            currentIndex += memory.Length;
        }

        return result;
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

    public static ReadOnlyMemory<TSource> OrderBy<TSource>(
        this ReadOnlyMemory<TSource> source,
        Func<TSource, uint> keySelector
    )
    {
        var result = new Memory<TSource>(new TSource[source.Length]);
        source.CopyTo(result);
        result.Span.BinarySort(keySelector);

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
