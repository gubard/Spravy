using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    public ForgotPasswordViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public ICommand ForgotPasswordCommand { get; protected set; }
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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        ForgotPasswordCommand = CreateCommandFromTask(TaskWork.Create(ForgotPasswordAsync).RunAsync);

        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return AuthenticationService.UpdateVerificationCodeByEmailAsync(Identifier, cancellationToken);
            case UserIdentifierType.Login:
                return AuthenticationService.UpdateVerificationCodeByLoginAsync(Identifier, cancellationToken);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse.IfSuccessAsync(
                () =>
                {
                    switch (IdentifierType)
                    {
                        case UserIdentifierType.Email:
                            return AuthenticationService.UpdatePasswordByEmailAsync(
                                Identifier,
                                VerificationCode.ToUpperInvariant(),
                                NewPassword,
                                cancellationToken
                            );
                        case UserIdentifierType.Login:
                            return AuthenticationService.UpdatePasswordByLoginAsync(
                                Identifier,
                                VerificationCode.ToUpperInvariant(),
                                NewPassword,
                                cancellationToken
                            );
                        default: throw new ArgumentOutOfRangeException();
                    }
                },
                cancellationToken
            )
            .IfSuccessAsync(() => Navigator.NavigateToAsync<LoginViewModel>(cancellationToken), cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
    }
}