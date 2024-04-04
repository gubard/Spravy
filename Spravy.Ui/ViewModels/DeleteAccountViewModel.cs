using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return Result.SuccessValueTask;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return Result.SuccessValueTask;
    }

    private ValueTask<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAsync(
                () =>
                {
                    switch (IdentifierType)
                    {
                        case UserIdentifierType.Email:
                            return AuthenticationService.DeleteUserByEmailAsync(
                                    Identifier,
                                    VerificationCode.ToUpperInvariant(),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                        case UserIdentifierType.Login:
                            return AuthenticationService.DeleteUserByEmailAsync(
                                    Identifier,
                                    VerificationCode.ToUpperInvariant(),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            )
            .ConfigureAwait(false)
            .IfSuccessAsync(() => Navigator.NavigateToAsync<LoginViewModel>(cancellationToken).ConfigureAwait(false));
    }

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
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