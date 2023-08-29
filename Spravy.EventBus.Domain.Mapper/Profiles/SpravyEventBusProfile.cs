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

        CreateMap<SubscribeEventsReply, EventValue>()
            .ConvertUsing(
                (source, _, resolutionContext) => new EventValue(
                    resolutionContext.Mapper.Map<Guid>(source.EventId),
                    new MemoryStream(source.Content.ToByteArray())
                )
            );
    }
}