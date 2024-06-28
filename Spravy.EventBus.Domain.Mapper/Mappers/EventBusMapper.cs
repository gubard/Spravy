using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class EventBusMapper
{
    public static partial ReadOnlyMemory<Event> ToEvent(this ReadOnlyMemory<EventValue> value);

    public static partial ReadOnlyMemory<EventValue> ToEventValue(this IEnumerable<Event> value);

    public static partial Event ToEvent(this EventValue value);

    public static partial SubscribeEventsReply ToSubscribeEventsReply(this EventValue value);

    public static EventValue ToEventValue(this SubscribeEventsReply value)
    {
        return new(value.EventId.ToGuid(), value.Content.ToByteMemory());
    }

    public static EventValue ToEventValue(this Event value)
    {
        return new(value.EventId.ToGuid(), value.Content.ToByteMemory());
    }

    private static ByteString ToByteString(ReadOnlyMemory<byte> request)
    {
        return request.ToByteString();
    }

    private static Guid ToGuid(ByteString value)
    {
        return value.ToGuid();
    }

    private static ReadOnlyMemory<byte> ToByteMemory(ByteString array)
    {
        return array.ToByteArray();
    }
}
