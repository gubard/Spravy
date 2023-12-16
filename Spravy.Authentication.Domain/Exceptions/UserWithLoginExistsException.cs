using Spravy.Domain.Exceptions;

namespace Spravy.Authentication.Domain.Exceptions;

public class UserWithLoginExistsException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("C60C2462-4436-4CBA-8E91-D121059A7C2C");

    public UserWithLoginExistsException(string login) : base(Identity, $"User with login {login} exits")
    {
        Login = login;
    }

    public string Login { get; }
}