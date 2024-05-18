using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class MemoryExtension
{
    public static ReadOnlyMemory<TResult> Select<TSource, TResult>(
        this ReadOnlyMemory<TSource> memory,
        Func<TSource, TResult> selector
    )
    {
        var result = new TResult[memory.Length];
        
        for (var index = 0; index < memory.Span.Length; index++)
        {
            result[index] = selector.Invoke(memory.Span[index]);
        }
        
        return result;
    }
    
    public static Result<T> GetSingle<T>(this ReadOnlyMemory<T> memory, string arrayName)
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