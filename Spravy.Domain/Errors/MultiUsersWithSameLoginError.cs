namespace Spravy.Domain.Errors;

public class MultiUsersWithSameLoginError : Error
{
    public static readonly Guid MainId = new("24DC9AB7-846B-4195-AD70-AB8296DD3412");

    protected MultiUsersWithSameLoginError() : base(MainId, "MultiUsersWithSameLoginError")
    {
    }

    public MultiUsersWithSameLoginError(string login) : base(MainId, "MultiUsersWithSameLoginError")
    {
        Login = login;
    }

    public string Login { get; protected set; }
}