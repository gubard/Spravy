using AutoMapper;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Db.Core.Profiles;

public class SpravyAuthenticationDbProfile : Profile
{
    public SpravyAuthenticationDbProfile()
    {
        CreateMap<UserEntity, UserTokenClaims>();
    }
}