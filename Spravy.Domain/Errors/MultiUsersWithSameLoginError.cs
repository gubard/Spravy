namespace Spravy.Domain.Errors;

public class MultiUsersWithSameLoginError : Error
{
    public static readonly Guid MainId = new("24DC9AB7-846B-4195-AD70-AB8296DD3412");

    protected MultiUsersWithSameLoginError() : base(MainId)
    {
        Login = string.Empty;
    }

    public MultiUsersWithSameLoginError(string login) : base(MainId)
    {
        Login = login;
    }

    public override string Message => $"Multi users with same login \"{Login}\"";

    public string Login { get; protected set; }
}