namespace Spravy.Domain.Exceptions;

public class ObjectDisposedException<T> : ObjectDisposedException where T : notnull
{
    public ObjectDisposedException(T value) : base(value.ToString())
    {
        Value = value;
    }

    public T Value { get; }
}