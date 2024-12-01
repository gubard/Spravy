namespace Spravy.Domain.Errors;

public class UserNotVerifiedError : Error
{
    public static readonly Guid MainId = new("CAAE89FE-827F-4770-B7A3-511ED6AB61CB");

    protected UserNotVerifiedError() : base(MainId)
    {
        Login = string.Empty;
        Email = string.Empty;
    }

    public UserNotVerifiedError(string login, string email) : base(MainId)
    {
        Login = login;
        Email = email;
    }

    public string Login { get; protected set; }
    public string Email { get; protected set; }

    public override string Message => $"User {Login}<{Email}> is not verified";
}

public class UserVerifiedError : Error
{
    public static readonly Guid MainId = new("3E08B5FD-6C08-4605-8A58-E580058F564D");

    protected UserVerifiedError() : base(MainId)
    {
        Login = string.Empty;
        Email = string.Empty;
    }

    public UserVerifiedError(string login, string email) : base(MainId)
    {
        Login = login;
        Email = email;
    }

    public string Login { get; protected set; }
    public string Email { get; protected set; }

    public override string Message => $"User {Login}<{Email}> is verified";
}