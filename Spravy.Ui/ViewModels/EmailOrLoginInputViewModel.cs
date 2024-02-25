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

    private async Task ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIBackgroundAsync(() => IsBusy = true);

        try
        {
            if (EmailOrLogin.Contains('@'))
            {
                if (await AuthenticationService.IsVerifiedByEmailAsync(EmailOrLogin, cancellationToken))
                {
                    await AuthenticationService.UpdateVerificationCodeByEmailAsync(EmailOrLogin, cancellationToken);
                }
                else
                {
                    await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                        vm =>
                        {
                            vm.IdentifierType = UserIdentifierType.Email;
                            vm.Identifier = EmailOrLogin;
                        },
                        cancellationToken
                    );

                    return;
                }
            }
            else
            {
                if (await AuthenticationService.IsVerifiedByLoginAsync(EmailOrLogin, cancellationToken))
                {
                    await AuthenticationService.UpdateVerificationCodeByLoginAsync(EmailOrLogin, cancellationToken);
                }
                else
                {
                    await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                        vm =>
                        {
                            vm.IdentifierType = UserIdentifierType.Login;
                            vm.Identifier = EmailOrLogin;
                        },
                        cancellationToken
                    );

                    return;
                }
            }

            await Navigator.NavigateToAsync<ForgotPasswordViewModel>(
                vm =>
                {
                    vm.Identifier = EmailOrLogin;

                    vm.IdentifierType = EmailOrLogin.Contains('@')
                        ? UserIdentifierType.Email
                        : UserIdentifierType.Login;
                },
                cancellationToken
            );
        }
        finally
        {
            await this.InvokeUIBackgroundAsync(() => IsBusy = false);
        }
    }

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(
            ViewId,
            new EmailOrLoginInputViewModelSetting(this)
        );
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<EmailOrLoginInputViewModelSetting>();

        await this.InvokeUIBackgroundAsync(() => EmailOrLogin = s.Identifier);
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