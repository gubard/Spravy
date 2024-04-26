namespace Spravy.Domain.Errors;

public class MultiUsersWithSameEmailError : Error
{
    public static readonly Guid MainId = new("0EDF06C1-2C51-4B8A-A19C-D423DCA0849C");

    protected MultiUsersWithSameEmailError() : base(MainId)
    {
        Email = string.Empty;
    }

    public MultiUsersWithSameEmailError(string email) : base(MainId)
    {
        Email = email;
    }

    public override string Message
    {
        get => $"Multi users with same email \"{Email}\"";
    }

    public string Email { get; protected set; }
}