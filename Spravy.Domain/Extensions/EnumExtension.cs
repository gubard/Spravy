namespace Spravy.Domain.Extensions;

public static class EnumExtension
{
    public static bool Contains<TEnum>(this ReadOnlySpan<TEnum> span, TEnum @enum)
        where TEnum : Enum
    {
        if (span.IsEmpty)
        {
            return false;
        }

        foreach (var e in span)
        {
            if (e.Equals(@enum))
            {
                return true;
            }
        }

        return false;
    }
}
