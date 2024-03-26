using AutoMapper;
using Google.Protobuf;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;

namespace Spravy.PasswordGenerator.Domain.Mapper.Profiles;

public class SpravyPasswordGeneratorProfile : Profile
{
    public SpravyPasswordGeneratorProfile()
    {
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
        CreateMap<ByteString, Guid>().ConstructUsing(x => new Guid(x.ToByteArray()));
        CreateMap<byte[], ByteString>().ConstructUsing(x => ByteString.CopyFrom(x));
        CreateMap<ByteString, byte[]>().ConstructUsing(x => x.ToByteArray());
        CreateMap<AddPasswordOptions, AddPasswordItemRequest>();
        CreateMap<AddPasswordItemRequest, AddPasswordOptions>();
        CreateMap<PasswordItem, PasswordItemGrpc>();
        CreateMap<PasswordItemGrpc, PasswordItem>();
    }
}