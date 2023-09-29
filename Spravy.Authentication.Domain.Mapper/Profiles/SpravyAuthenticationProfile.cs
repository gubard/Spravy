using AutoMapper;
using Google.Protobuf;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Mapper.Profiles;

public class SpravyAuthenticationProfile : Profile
{
    public SpravyAuthenticationProfile()
    {
        CreateMap<User, UserGrpc>();
        CreateMap<CreateUserOptions, CreateUserRequest>();
        CreateMap<CreateUserRequest, CreateUserOptions>();
        CreateMap<UserGrpc, User>();
        CreateMap<TokenResult, LoginReply>();
        CreateMap<LoginReply, TokenResult>();
        CreateMap<TokenResult, RefreshTokenReply>();
        CreateMap<RefreshTokenReply, TokenResult>();
        CreateMap<Guid, ByteString>().ConvertUsing(x => ByteString.CopyFrom(x.ToByteArray()));
    }
}