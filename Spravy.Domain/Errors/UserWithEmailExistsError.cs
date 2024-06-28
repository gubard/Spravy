namespace Spravy.Domain.Errors;

public class UserWithEmailExistsError : Error
{
    public static readonly Guid MainId = new("2759AC7A-45D9-4D3F-9762-C145568C5F00");

    protected UserWithEmailExistsError()
        : base(MainId)
    {
        Email = string.Empty;
    }

    public UserWithEmailExistsError(string email)
        : base(MainId)
    {
        Email = email;
    }

    public string Email { get; protected set; }

    public override string Message
    {
        get => $"User with email \"{Email}\" already exists";
    }
}
