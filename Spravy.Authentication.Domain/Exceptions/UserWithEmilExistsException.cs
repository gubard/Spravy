using Spravy.Domain.Exceptions;

namespace Spravy.Authentication.Domain.Exceptions;

public class UserWithEmilExistsException : SpravyException
{
    public static readonly Guid Identity = Guid.Parse("C9384E70-55D9-49B4-BCB9-9D9975886C65");

    public UserWithEmilExistsException(string email) : base(Identity, $"User with email {email} exits")
    {
        Email = email;
    }

    public string Email { get; }
}