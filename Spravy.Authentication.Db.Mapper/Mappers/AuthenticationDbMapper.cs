using Riok.Mapperly.Abstractions;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class AuthenticationDbMapper
{
    public static UserTokenClaims ToUserTokenClaims(this UserEntity entity)
    {
        return new(entity.Login ?? string.Empty, entity.Id, entity.Role, entity.Email ?? string.Empty);
    }
}