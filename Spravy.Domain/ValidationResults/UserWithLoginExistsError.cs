namespace Spravy.Domain.ValidationResults;

public class UserWithLoginExistsError : Error
{
    public static readonly Guid MainId = new("A5D95424-DEFF-4571-A740-E34E42B34DD8");
    
    public UserWithLoginExistsError(string login) : base(MainId, "UserWithLoginExists")
    {
        Login = login;
    }

    public string Login { get; protected set; }
}