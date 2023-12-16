using Spravy.Domain.Exceptions;

namespace Spravy.Authentication.Domain.Exceptions;

public class UserWrongPasswordException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("9EA21965-BFF3-486D-B850-BCB3C92D62FD");
    
    public UserWrongPasswordException(string login) : base(Identity, $"Wrong password for user {login}")
    {
        Login = login;
    }
    
    public string Login { get; }
}