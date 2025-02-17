using Google.Protobuf.WellKnownTypes;

namespace Spravy.Schedule.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ScheduleMapper
{
    public static partial TimerItem ToTimerItem(this TimerItemGrpc grpc);

    public static partial TimerItemGrpc ToTimerItemGrpc(this TimerItem grpc);

    public static partial AddTimerParameters ToAddTimerParameters(this AddTimerParametersGrpc grpc);

    public static partial AddTimerParametersGrpc ToAddTimerRequestGrpc(this AddTimerParameters grpc);

    public static partial ReadOnlyMemory<TimerItem> ToTimerItem(this IEnumerable<TimerItemGrpc> grpc);

    public static partial ReadOnlyMemory<TimerItemGrpc> ToTimerItemGrpc(this ReadOnlyMemory<TimerItem> grpc);

    public static Timestamp ToTimestamp(DateTime dateTime)
    {
        return dateTime.MapToTimestamp();
    }

    public static DateTime ToDateTime(Timestamp dateTime)
    {
        return dateTime.MapToDateTime();
    }

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }

    private static Guid ToGuid(ByteString byteString)
    {
        return byteString.ToGuid();
    }

    private static ReadOnlyMemory<byte> ToByteMemory(ByteString array)
    {
        return array.ToByteMemory();
    }

    private static ByteString ToByteString(ReadOnlyMemory<byte> request)
    {
        return request.ToByteString();
    }
}