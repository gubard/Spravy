using AutoMapper;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Db.Mapper.Profiles;

public class SpravyAuthenticationDbProfile : Profile
{
    public SpravyAuthenticationDbProfile()
    {
        CreateMap<UserEntity, TokenClaims>();
    }
}