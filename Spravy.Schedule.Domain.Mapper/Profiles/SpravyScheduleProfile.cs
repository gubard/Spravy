using AutoMapper;
using Google.Protobuf;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Domain.Mapper.Profiles;

public class SpravyScheduleProfile : Profile
{
    public SpravyScheduleProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<Stream, ByteString>().ConvertUsing(x => ByteString.FromStream(x));
        CreateMap<ByteString, Stream>().ConstructUsing(x => new MemoryStream(x.ToByteArray()));
        CreateMap<TimerItem, TimerItemGrpc>();
        CreateMap<TimerItemGrpc, TimerItem>();
        CreateMap<AddTimerParametersGrpc, AddTimerParameters>();
        CreateMap<AddTimerParameters, AddTimerParametersGrpc>();
    }
}