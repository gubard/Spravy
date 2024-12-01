namespace Spravy.Domain.Errors;

public class VerificationCodePasswordError : Error
{
    public static readonly Guid MainId = new("647B9105-CCA1-4EFD-B640-014AE582A33E");

    public VerificationCodePasswordError() : base(MainId)
    {
    }

    public override string Message => "Wrong verification code";
}