namespace Spravy.Domain.Errors;

public class UserWithEmailExistsError : Error
{
    public static readonly Guid MainId = new("2759AC7A-45D9-4D3F-9762-C145568C5F00");

    public UserWithEmailExistsError() : base(MainId)
    {
    }

    public string Email { get; protected set; }

    public override string Message
    {
        get => $"User with email \"{Email}\" already exists";
    }
}