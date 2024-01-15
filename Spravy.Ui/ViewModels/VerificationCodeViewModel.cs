using ReactiveUI;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class VerificationCodeViewModel : ViewModelBase, IVerificationEmail
{
    private string identifier = string.Empty;
    private UserIdentifierType identifierType;
    private string verificationCode = string.Empty;

    public string Identifier
    {
        get => identifier;
        set => this.RaiseAndSetIfChanged(ref identifier, value);
    }

    public UserIdentifierType IdentifierType
    {
        get => identifierType;
        set => this.RaiseAndSetIfChanged(ref identifierType, value);
    }

    public string VerificationCode
    {
        get => verificationCode;
        set =>this.RaiseAndSetIfChanged(ref verificationCode, value);
    }
}