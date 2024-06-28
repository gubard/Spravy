namespace Spravy.Domain.Errors;

public class WrongPassword : Error
{
    public static readonly Guid MainId = new("4F8E8DED-9D5C-4BCD-9AB1-71AE10B85643");

    public WrongPassword()
        : base(MainId) { }

    public override string Message
    {
        get => "Wrong password";
    }
}
