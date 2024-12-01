namespace Spravy.Domain.Models;

public class Ref<T> where T : struct
{
    public Ref(T value)
    {
        Value = value;
    }

    public T Value { get; }
}