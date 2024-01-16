using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
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
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public override string ViewId => TypeCache<VerificationCodeViewModel>.Type.Name;

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

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

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return AuthenticationService.UpdateVerificationCodeByEmailAsync(Identifier, cancellationToken);
            case UserIdentifierType.Login:
                return AuthenticationService.UpdateVerificationCodeByLoginAsync(Identifier, cancellationToken);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}