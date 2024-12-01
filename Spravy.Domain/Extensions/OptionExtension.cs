namespace Spravy.Domain.Extensions;

public static class OptionExtension
{
    public static T? ToNullable<T>(this OptionStruct<T> option) where T : struct
    {
        return option.TryGetValue(out var value) ? value : null;
    }
}