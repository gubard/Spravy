namespace Spravy.Domain.Errors;

public class UserWithLoginExistsError : Error
{
    public static readonly Guid MainId = new("A5D95424-DEFF-4571-A740-E34E42B34DD8");

    protected UserWithLoginExistsError()
        : base(MainId)
    {
        Login = string.Empty;
    }

    public UserWithLoginExistsError(string login)
        : base(MainId)
    {
        Login = login;
    }

    public string Login { get; protected set; }

    public override string Message
    {
        get => $"User with login \"{Login}\" already exists";
    }
}
