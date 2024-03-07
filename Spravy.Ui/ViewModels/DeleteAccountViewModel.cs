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
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class DeleteAccountViewModel : NavigatableViewModelBase
{
    public DeleteAccountViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        DeleteAccountCommand = CreateInitializedCommand(TaskWork.Create(DeleteAccountAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }

    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;

    [Reactive]
    public string Identifier { get; set; } = string.Empty;

    public override string ViewId => TypeCache<DeleteAccountViewModel>.Type.Name;
    public ICommand InitializedCommand { get; }
    public ICommand DeleteAccountCommand { get; }

    public override void Stop()
    {
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    private async Task DeleteAccountAsync(CancellationToken cancellationToken)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                await AuthenticationService.DeleteUserByEmailAsync(
                    Identifier,
                    VerificationCode.ToUpperInvariant(),
                    cancellationToken
                );
                break;
            case UserIdentifierType.Login:
                await AuthenticationService.DeleteUserByEmailAsync(
                    Identifier,
                    VerificationCode.ToUpperInvariant(),
                    cancellationToken
                );
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        await Navigator.NavigateToAsync<LoginViewModel>(cancellationToken);
    }

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
}