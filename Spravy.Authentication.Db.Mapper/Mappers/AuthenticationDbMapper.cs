using Riok.Mapperly.Abstractions;
using Spravy.Authentication.Db.Models;
using Spravy.Authentication.Domain.Models;

namespace Spravy.Authentication.Db.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class AuthenticationDbMapper
{
    public static partial UserTokenClaims ToUserTokenClaims(this UserEntity entity);
}
