namespace Spravy.Ui.Interfaces;

public interface IVerificationEmail
{
    public string Identifier { get; }
    public UserIdentifierType IdentifierType { get; }
    public string VerificationCode { get; }
}
