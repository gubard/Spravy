using AutoMapper;
using Google.Protobuf;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Domain.Mapper.Profiles;

public class SpravyEventBusProfile : Profile
{
    public SpravyEventBusProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new(x.ToByteArray()));
        CreateMap<byte[], ByteString>().ConstructUsing(x => ByteString.CopyFrom(x));
        CreateMap<ByteString, byte[]>().ConstructUsing(x => x.ToByteArray());

        CreateMap<EventValue, Event>()
           .ConstructUsing((x, context) => new()
            {
                EventId = context.Mapper.Map<ByteString>(x.Id),
                Content = context.Mapper.Map<ByteString>(x.Content),
            });

        CreateMap<Event, EventValue>()
           .ConstructUsing((x, context) => new(context.Mapper.Map<Guid>(x.EventId), x.Content.ToByteArray()));

        CreateMap<SubscribeEventsReply, EventValue>()
           .ConstructUsing((x, context) => new(context.Mapper.Map<Guid>(x.EventId), x.Content.ToByteArray()));
    }
}