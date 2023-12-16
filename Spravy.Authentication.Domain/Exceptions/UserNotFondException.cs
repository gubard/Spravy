using Spravy.Domain.Exceptions;

namespace Spravy.Authentication.Domain.Exceptions;

public class UserNotFondException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("CDDEBD97-CB48-4954-BF72-DBB6F0717827");

    public UserNotFondException(string login) : base(Identity, $"User with login {login} not found")
    {
        Login = login;
    }

    public string Login { get; }
}