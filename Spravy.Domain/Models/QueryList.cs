using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public class QueryList<T> where T : class
{
    private readonly T?[] array;
    private readonly ulong size;

    public QueryList(ulong size)
    {
        this.size = size;
        array = new T[size];
    }

    public QueryList<T> Add(T item)
    {
        item.ThrowIfNull();

        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                continue;
            }

            array[i] = item;

            return this;
        }

        for (var i = 0; i < array.Length - 1; i++)
        {
            array[i] = array[i + 1];
        }

        array[^1] = item;

        return this;
    }

    public T? Pop()
    {
        var index = 0;

        for (var i = 1; i < array.Length - 1; i++)
        {
            if (array[i] != null)
            {
                index = i;
            }
            else
            {
                break;
            }
        }

        var result = array[index];
        array[index] = default;

        return result;
    }
}