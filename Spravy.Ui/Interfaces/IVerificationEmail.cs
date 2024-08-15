namespace Spravy.Ui.Interfaces;

public interface IVerificationEmail
{
    public string EmailOrLogin { get; }
    public UserIdentifierType IdentifierType { get; }
    public string VerificationCode { get; }
}
