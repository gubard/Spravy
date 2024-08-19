using Google.Protobuf;
using Google.Protobuf.Collections;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class EventBusMapper
{
    public static Event ToEvent(this EventValue value)
    {
        return new() { EventId = value.Id.ToByteString(), Content = value.Content.ToByteString(), };
    }

    public static ReadOnlyMemory<Event> ToEvent(this ReadOnlyMemory<EventValue> value)
    {
        Memory<Event> memory = new Event[value.Length];

        for (var i = 0; i < value.Length; i++)
        {
            memory.Span[i] = value.Span[i].ToEvent();
        }

        return memory;
    }

    public static ReadOnlyMemory<EventValue> ToEventValue(this RepeatedField<Event> value)
    {
        Memory<EventValue> memory = new EventValue[value.Count];

        for (var i = 0; i < value.Count; i++)
        {
            memory.Span[i] = value[i].ToEventValue();
        }

        return memory;
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
