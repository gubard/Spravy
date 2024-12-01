using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Core.Mappers;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class AuthenticationMapper
{
    public static UserGrpc ToUserGrpc(this User user)
    {
        return new()
        {
            Login = user.Login,
            Password = user.Password,
        };
    }

    public static User ToUser(this UserGrpc user)
    {
        return new(user.Login, user.Password);
    }

    public static partial CreateUserRequest ToCreateUserRequest(this CreateUserOptions user);

    public static partial CreateUserOptions ToCreateUserOptions(this CreateUserRequest user);

    public static partial LoginReply ToLoginReply(this TokenResult user);

    public static partial TokenResult ToTokenResult(this LoginReply user);

    public static partial RefreshTokenReply ToRefreshTokenReply(this TokenResult user);

    public static partial TokenResult ToTokenResult(this RefreshTokenReply user);

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }
}