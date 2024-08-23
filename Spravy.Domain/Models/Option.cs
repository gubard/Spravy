namespace Spravy.Domain.Models;

public readonly struct Option<TValue>
    where TValue : class
{
    private readonly TValue? value;

    public Option()
    {
        value = null;
        IsHasValue = false;
    }

    public Option(TValue value)
    {
        this.value = value;
        IsHasValue = true;
    }

    public bool IsHasValue { get; }

    public Result<TValue> GetValue()
    {
        if (IsHasValue)
        {
            return new(value!);
        }

        return new(new PropertyNullValueError("Value"));
    }

    public TValue? GetNullable()
    {
        return value;
    }

    public bool TryGetValue([MaybeNullWhen(false)] out TValue result)
    {
        if (IsHasValue)
        {
            result = value!;

            return true;
        }

        result = null;

        return false;
    }
}
