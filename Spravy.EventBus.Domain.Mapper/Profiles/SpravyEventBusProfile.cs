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
        CreateMap<ByteString, Guid>().ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<byte[], ByteString>().ConstructUsing(x => ByteString.CopyFrom(x));
        CreateMap<ByteString, byte[]>().ConstructUsing(x => x.ToByteArray());

        CreateMap<SubscribeEventsReply, EventValue>()
            .ConstructUsing(
                (x, context) => new EventValue(context.Mapper.Map<Guid>(x.EventId), x.Content.ToByteArray())
            );
    }
}