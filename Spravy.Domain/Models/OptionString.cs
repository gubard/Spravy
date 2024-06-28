namespace Spravy.Domain.Models;

public readonly struct OptionString
{
    public OptionString(string? value)
    {
        Value = value;
        IsNullOrWhiteSpace = value.IsNullOrWhiteSpace();

        if (value is null)
        {
            IsNormalized = false;
            IsHasValue = false;
        }
        else
        {
            IsNormalized = value.IsNormalized();
            IsHasValue = true;
        }
    }

    public string? Value { get; }
    public bool IsNullOrWhiteSpace { get; }
    public bool IsHasValue { get; }
    public bool IsNormalized { get; }

    public bool TryGetValue([MaybeNullWhen(false)] out string value)
    {
        if (IsHasValue)
        {
            value = Value!;

            return true;
        }

        value = null;

        return false;
    }
}
