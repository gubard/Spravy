using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    public ForgotPasswordViewModel() : base(true)
    {
        ForgotPasswordCommand = CreateCommandFromTask(TaskWork.Create(ForgotPasswordAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public ICommand ForgotPasswordCommand { get; }
    public override string ViewId => TypeCache<ForgotPasswordViewModel>.Type.Name;
    public ICommand InitializedCommand { get; }

    [Reactive]
    public string Identifier { get; set; } = string.Empty;

    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }

    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;

    [Reactive]
    public string NewPassword { get; set; } = string.Empty;

    [Reactive]
    public string NewRepeatPassword { get; set; } = string.Empty;

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return AuthenticationService.UpdateVerificationCodeByEmailAsync(Identifier, cancellationToken);
            case UserIdentifierType.Login:
                return AuthenticationService.UpdateVerificationCodeByLoginAsync(Identifier, cancellationToken);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                await AuthenticationService.UpdatePasswordByEmailAsync(
                    Identifier,
                    VerificationCode.ToUpperInvariant(),
                    NewPassword,
                    cancellationToken
                );
                break;
            case UserIdentifierType.Login:
                await AuthenticationService.UpdatePasswordByLoginAsync(
                    Identifier,
                    VerificationCode.ToUpperInvariant(),
                    NewPassword,
                    cancellationToken
                );
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        await Navigator.NavigateToAsync<LoginViewModel>(cancellationToken);
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