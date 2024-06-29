namespace Spravy.Domain.Models;

public class QueryList<T>
    where T : class
{
    private Memory<T?> array;
    private int currentIndex;

    public QueryList(ulong size)
    {
        currentIndex = 0;
        array = new T[size];
    }

    public Result Add(T item)
    {
        return item.IfNotNull(nameof(item))
            .IfSuccess(i =>
            {
                if (array.Length == currentIndex)
                {
                    var slice = array.Slice(1, array.Length - 1);
                    slice.CopyTo(array);
                    array.Span[^1] = i;
                }
                else
                {
                    array.Span[currentIndex] = i;
                    currentIndex++;
                }

                return Result.Success;
            });
    }

    public Result<T> Pop()
    {
        if (array.IsEmpty)
        {
            return new(new EmptyArrayError("query"));
        }

        currentIndex--;

        return array.Span[currentIndex].ThrowIfNull().ToResult();
    }
}
