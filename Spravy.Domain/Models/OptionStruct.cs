namespace Spravy.Domain.Models;

public readonly struct OptionStruct<TValue> where TValue : struct
{
    public static OptionStruct<TValue> Default = new();

    private readonly TValue value;

    public OptionStruct(TValue value)
    {
        this.value = value;
        IsHasValue = true;
    }

    public bool IsHasValue { get; }

    public Result<TValue> GetValue()
    {
        if (IsHasValue)
        {
            return new(value);
        }

        return new(new PropertyNullValueError("Value"));
    }

    public bool TryGetValue(out TValue result)
    {
        result = value;

        return IsHasValue;
    }

    public TValue? GetValueOrNull()
    {
        if (IsHasValue)
        {
            return value;
        }

        return null;
    }
}