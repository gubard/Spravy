namespace Spravy.Domain.Errors;

public class ContinueError : Error
{
    public static readonly Guid MainId = new("7BA0E760-7234-459C-841E-2FA2AAD951FE");

    public ContinueError() : base(MainId)
    {
    }

    public override string Message => "Continue";
}