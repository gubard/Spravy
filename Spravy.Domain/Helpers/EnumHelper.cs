namespace Spravy.Domain.Helpers;

public static class EnumHelper
{
    public static TEnum MinByte<TEnum>(TEnum x, TEnum y)
        where TEnum : Enum
    {
        return Math.Min(x.ThrowIfIsNotCast<byte>(), y.ThrowIfIsNotCast<byte>())
            .ThrowIfIsNotCast<TEnum>();
    }
}
