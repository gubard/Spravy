using Spravy.Domain.Errors;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class MemoryExtension
{
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