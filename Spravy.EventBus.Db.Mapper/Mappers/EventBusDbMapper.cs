using Riok.Mapperly.Abstractions;
using Spravy.EventBus.Db.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class EventBusDbMapper
{
    public static EventValue ToEventValue(this EventEntity entity)
    {
        return new(entity.EventId, entity.Content);
    }
}
