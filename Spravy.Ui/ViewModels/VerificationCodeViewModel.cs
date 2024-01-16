using System.Threading.Tasks;
using ReactiveUI;
using Spravy.Domain.Helpers;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class VerificationCodeViewModel : NavigatableViewModelBase, IVerificationEmail
{
    private string identifier = string.Empty;
    private UserIdentifierType identifierType;
    private string verificationCode = string.Empty;

    public VerificationCodeViewModel() : base(true)
    {
    }

    public override string ViewId => TypeCache<VerificationCodeViewModel>.Type.Name;

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
        set => this.RaiseAndSetIfChanged(ref verificationCode, value);
    }

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }
}