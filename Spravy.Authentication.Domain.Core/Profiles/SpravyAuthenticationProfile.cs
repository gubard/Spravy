using AutoMapper;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;

namespace Spravy.Authentication.Domain.Core.Profiles;

public class SpravyAuthenticationProfile : Profile
{
    public SpravyAuthenticationProfile()
    {
        CreateMap<User, UserGrpc>();
        CreateMap<CreateUserOptions, CreateUserRequest>();
        CreateMap<CreateUserRequest, CreateUserOptions>();
        CreateMap<UserGrpc, User>().ConstructUsing(x => new(x.Login, x.Password));
    }
}