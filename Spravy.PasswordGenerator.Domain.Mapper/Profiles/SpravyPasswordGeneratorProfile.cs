using AutoMapper;
using Google.Protobuf;

namespace Spravy.PasswordGenerator.Domain.Mapper.Profiles;

public class SpravyPasswordGeneratorProfile : Profile
{
    public SpravyPasswordGeneratorProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<byte[], ByteString>().ConstructUsing(x => ByteString.CopyFrom(x));
        CreateMap<ByteString, byte[]>().ConstructUsing(x => x.ToByteArray());
    }
}