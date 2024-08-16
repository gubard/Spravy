using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Schedule.Db.Models;
using Spravy.Schedule.Domain.Models;

namespace Spravy.Schedule.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class ScheduleDbMapper
{
    public static partial ReadOnlyMemory<TimerItem> ToTimerItem(
        this ReadOnlyMemory<TimerEntity> entity
    );

    public static partial TimerEntity ToTimerEntity(this TimerItem entity);

    public static TimerItem ToTimerItem(this TimerEntity entity)
    {
        return new(entity.DueDateTime, entity.EventId, entity.Content, entity.Id, entity.Name);
    }

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }

    private static Guid ToGuid(ByteString byteString)
    {
        return byteString.ToGuid();
    }

    private static ReadOnlyMemory<byte> ToMemory(byte[] id)
    {
        return id.ToMemory();
    }
}
