namespace Spravy.Domain.Errors;

public class UserNotVerifiedError : Error
{
    public static readonly Guid MainId = new("CAAE89FE-827F-4770-B7A3-511ED6AB61CB");

    protected UserNotVerifiedError() : base(MainId, "UserNotVerifiedError")
    {
    }

    public UserNotVerifiedError(string login, string email) : base(MainId, "UserNotVerifiedError")
    {
        Login = login;
        Email = email;
    }

    public string Login { get; protected set; }
    public string Email { get; protected set; }
}