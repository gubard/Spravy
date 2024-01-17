using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
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
    private readonly TextViewModel textViewModel;

    public EmailOrLoginInputViewModel() : base(true)
    {
        ForgotPasswordCommand = CreateCommandFromTask(TaskWork.Create(ForgotPasswordAsync).RunAsync);
    }

    public override string ViewId => TypeCache<EmailOrLoginInputViewModel>.Type.Name;

    [Inject]
    public required TextViewModel TextViewModel
    {
        get => textViewModel;
        [MemberNotNull(nameof(textViewModel))]
        init
        {
            textViewModel = value;
            textViewModel.Label = "EmailOrLogin";
        }
    }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public ICommand ForgotPasswordCommand { get; }

    private async Task ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        if (TextViewModel.Text.Contains('@'))
        {
            if (await AuthenticationService.IsVerifiedByEmailAsync(TextViewModel.Text, cancellationToken))
            {
                await AuthenticationService.UpdateVerificationCodeByEmailAsync(TextViewModel.Text, cancellationToken);
            }
            else
            {
                await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                    vm =>
                    {
                        vm.IdentifierType = UserIdentifierType.Email;
                        vm.Identifier = TextViewModel.Text;
                    },
                    cancellationToken
                );
                
                return;
            }
        }
        else
        {
            if (await AuthenticationService.IsVerifiedByLoginAsync(TextViewModel.Text, cancellationToken))
            {
                await AuthenticationService.UpdateVerificationCodeByLoginAsync(TextViewModel.Text, cancellationToken);
            }
            else
            {
                await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                    vm =>
                    {
                        vm.IdentifierType = UserIdentifierType.Login;
                        vm.Identifier = TextViewModel.Text;
                    },
                    cancellationToken
                );
                
                return;
            }
        }

        await Navigator.NavigateToAsync<ForgotPasswordViewModel>(
            vm =>
            {
                vm.Identifier = TextViewModel.Text;

                vm.IdentifierType = TextViewModel.Text.Contains('@')
                    ? UserIdentifierType.Email
                    : UserIdentifierType.Login;
            },
            cancellationToken
        );
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

        await this.InvokeUIBackgroundAsync(() => TextViewModel.Text = s.Identifier);
    }

    [ProtoContract]
    class EmailOrLoginInputViewModelSetting
    {
        public EmailOrLoginInputViewModelSetting()
        {
        }

        public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
        {
            Identifier = viewModel.TextViewModel.Text;
        }

        [ProtoMember(1)]
        public string Identifier { get; set; } = string.Empty;
    }
}