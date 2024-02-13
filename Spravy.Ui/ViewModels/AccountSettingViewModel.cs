using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class AccountSettingViewModel : NavigatableViewModelBase
{
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public AccountSettingViewModel() : base(true)
    {
        ChangePasswordCommand = CreateCommandFromTask(TaskWork.Create(ChangePasswordAsync).RunAsync);
    }

    public override string ViewId => TypeCache<AccountSettingViewModel>.Type.Name;
    public ICommand ChangePasswordCommand { get; }
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

    [Inject]
    public required AccountNotify AccountNotify { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Settings";
        }
    }

    private async Task ChangePasswordAsync(CancellationToken cancellationToken)
    {
        await AuthenticationService.UpdateVerificationCodeByLoginAsync(AccountNotify.Login, cancellationToken)
            .ConfigureAwait(false);

        await Navigator.NavigateToAsync<ForgotPasswordViewModel>(
                vm =>
                {
                    vm.Identifier = AccountNotify.Login;
                    vm.IdentifierType = UserIdentifierType.Login;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
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