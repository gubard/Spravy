using Google.Protobuf.WellKnownTypes;

namespace Spravy.Core.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class CoreMapper
{
    public static partial ReadOnlyMemory<ByteString> ToByteString(this ReadOnlyMemory<Guid> value);

    public static partial ReadOnlyMemory<Guid> ToGuid(this IEnumerable<ByteString> value);

    public static DateTime ToDateTime(this DateOnly value)
    {
        return new(value.Year, value.Month, value.Day);
    }

    public static DateOnly ToDateOnly(this DateTime value)
    {
        return new(value.Year, value.Month, value.Day);
    }

    public static Timestamp ToTimestamp(this DateOnly value)
    {
        return Timestamp.FromDateTime(value.ToDateTime(DateTimeKind.Utc));
    }

    public static DateOnly ToDateOnly(this Timestamp value)
    {
        return value.ToDateTime().ToDateOnly();
    }

    public static Timestamp MapToTimestamp(this DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Unspecified => Timestamp.FromDateTime(
                TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.Local)
            ),
            DateTimeKind.Utc => Timestamp.FromDateTime(value),
            DateTimeKind.Local => Timestamp.FromDateTime(TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.Local)),
            _ => throw new ArgumentOutOfRangeException(nameof(value.Kind), value.Kind.ToString()),
        };
    }

    public static DateTime MapToDateTime(this Timestamp value)
    {
        return value.ToDateTime();
    }

    public static Option<Uri> ToOptionUri(this string? value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return new();
        }

        return new(value.ToUri());
    }

    public static string MapToString(this Option<Uri> value)
    {
        if (value.TryGetValue(out var uri))
        {
            return uri.AbsoluteUri;
        }

        return string.Empty;
    }

    public static ReadOnlyMemory<byte> ToByteMemory(this ByteString value)
    {
        return value.ToByteArray();
    }

    public static ByteString ToByteString(this ReadOnlyMemory<byte> value)
    {
        return ByteString.CopyFrom(value.Span);
    }

    public static ByteString ToByteString(this Guid value)
    {
        return ByteString.CopyFrom(value.ToByteArray());
    }

    public static Guid ToGuid(this ByteString value)
    {
        return new(value.ToByteArray());
    }

    public static byte[] ToArrayByte(this Guid value)
    {
        return value.ToByteArray();
    }

    public static Guid ToGuid(this byte[] value)
    {
        return new(value);
    }

    public static ReadOnlyMemory<byte> ToMemory(this byte[] value)
    {
        return value;
    }

    public static ByteString ToByteString(this OptionStruct<Guid> value)
    {
        if (value.TryGetValue(out var id))
        {
            return id.ToByteString();
        }

        return ByteString.Empty;
    }

    public static OptionStruct<Guid> ToOptionGuid(this ByteString value)
    {
        if (value.IsEmpty)
        {
            return new();
        }

        return new(value.ToGuid());
    }

    public static Guid? ToNullableGuid(this OptionStruct<Guid> value)
    {
        return value.TryGetValue(out var v) ? v : null;
    }

    public static OptionStruct<Guid> ToOptionGuid(this Guid? value)
    {
        return value.ToOption();
    }
}