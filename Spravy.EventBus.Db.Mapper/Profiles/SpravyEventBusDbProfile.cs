using AutoMapper;
using Spravy.EventBus.Db.Models;
using Spravy.EventBus.Domain.Models;

namespace Spravy.EventBus.Db.Mapper.Profiles;

public class SpravyEventBusDbProfile : Profile
{
    public SpravyEventBusDbProfile()
    {
        CreateMap<EventEntity, EventValue>().ConstructUsing(x => new EventValue(x.EventId, x.Content));
    }
}