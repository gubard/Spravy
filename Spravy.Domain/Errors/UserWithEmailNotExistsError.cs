namespace Spravy.Domain.Errors;

public class UserWithEmailNotExistsError : Error
{
    public static readonly Guid MainId = new("8C3C0491-09FB-48EC-AE28-2E40B51513A0");

    protected UserWithEmailNotExistsError() : base(MainId)
    {
        Email = string.Empty;
    }

    public UserWithEmailNotExistsError(string email) : base(MainId)
    {
        Email = email;
    }

    public string Email { get; protected set; }

    public override string Message => $"User with email \"{Email}\" not exists";
}