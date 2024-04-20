namespace Spravy.Domain.Errors;

public class UserWithLoginNotExistsError : Error
{
    public static readonly Guid MainId = new("61C6FDA6-95AD-47BE-995B-BDA1147FE9A3");

    protected UserWithLoginNotExistsError() : base(MainId, "UserWithLoginNotExists")
    {
        Login = string.Empty;
    }

    public UserWithLoginNotExistsError(string login) : base(MainId, "UserWithLoginNotExists")
    {
        Login = login;
    }

    public string Login { get; protected set; }
}