namespace Spravy.Domain.Errors;

public class WrongPasswordError : Error
{
    public static readonly Guid MainId = new("4F8E8DED-9D5C-4BCD-9AB1-71AE10B85643");

    public WrongPasswordError() : base(MainId)
    {
    }

    public override string Message => "Wrong password";
}