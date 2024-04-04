using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class EmailOrLoginInputViewModel : NavigatableViewModelBase
{
    public EmailOrLoginInputViewModel() : base(true)
    {
        ForgotPasswordCommand = CreateCommandFromTask(TaskWork.Create(ForgotPasswordAsync).RunAsync);
    }

    public override string ViewId => TypeCache<EmailOrLoginInputViewModel>.Type.Name;

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public ICommand ForgotPasswordCommand { get; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public string EmailOrLogin { get; set; } = string.Empty;

    private ValueTask<Result> ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .ConfigureAwait(false)
            .IfSuccessTryFinallyAsync(
                () =>
                {
                      if (EmailOrLogin.Contains('@'))
        {
            return AuthenticationService.IsVerifiedByEmailAsync(EmailOrLogin, cancellationToken)
                .ConfigureAwait(false)
                .IfSuccessAsync(
                    value =>
                    {
                        if (value)
                        {
                            return AuthenticationService.UpdateVerificationCodeByEmailAsync(
                                    EmailOrLogin,
                                    cancellationToken
                                )
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    () => Navigator.NavigateToAsync<ForgotPasswordViewModel>(
                                            vm =>
                                            {
                                                vm.Identifier = EmailOrLogin;

                                                vm.IdentifierType = EmailOrLogin.Contains('@')
                                                    ? UserIdentifierType.Email
                                                    : UserIdentifierType.Login;
                                            },
                                            cancellationToken
                                        )
                                        .ConfigureAwait(false)
                                )
                                .ConfigureAwait(false);
                        }

                        return Navigator.NavigateToAsync<VerificationCodeViewModel>(
                                vm =>
                                {
                                    vm.IdentifierType = UserIdentifierType.Email;
                                    vm.Identifier = EmailOrLogin;
                                },
                                cancellationToken
                            )
                            .ConfigureAwait(false);
                    }
                )
                .ConfigureAwait(false);
        }

        return AuthenticationService.IsVerifiedByLoginAsync(EmailOrLogin, cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                value =>
                {
                    if (value)
                    {
                        return AuthenticationService
                            .UpdateVerificationCodeByLoginAsync(EmailOrLogin, cancellationToken)
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                () => Navigator.NavigateToAsync<ForgotPasswordViewModel>(
                                        vm =>
                                        {
                                            vm.Identifier = EmailOrLogin;

                                            vm.IdentifierType = EmailOrLogin.Contains('@')
                                                ? UserIdentifierType.Email
                                                : UserIdentifierType.Login;
                                        },
                                        cancellationToken
                                    )
                                    .ConfigureAwait(false)
                            )
                            .ConfigureAwait(false);
                    }

                    return Navigator.NavigateToAsync<VerificationCodeViewModel>(
                            vm =>
                            {
                                vm.IdentifierType = UserIdentifierType.Login;
                                vm.Identifier = EmailOrLogin;
                            },
                            cancellationToken
                        )
                        .ConfigureAwait(false);
                }
            )
            .ConfigureAwait(false);
                },
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ConfigureAwait(false)
                    .ToValueTask()
                    .ConfigureAwait(false)
            );
        ;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(
            ViewId,
            new EmailOrLoginInputViewModelSetting(this)
        );
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<EmailOrLoginInputViewModelSetting>();

        return this.InvokeUIBackgroundAsync(() => EmailOrLogin = s.Identifier);
    }

    [ProtoContract]
    class EmailOrLoginInputViewModelSetting
    {
        public EmailOrLoginInputViewModelSetting()
        {
        }

        public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
        {
            Identifier = viewModel.EmailOrLogin;
        }

        [ProtoMember(1)]
        public string Identifier { get; set; } = string.Empty;
    }
}