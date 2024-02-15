using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Styling;
using Ninject;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Models;
using SukiUI;
using SukiUI.Models;

namespace Spravy.Ui.ViewModels;

public class AccountSettingViewModel : NavigatableViewModelBase
{
    private readonly SukiTheme theme = SukiTheme.GetInstance();
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private bool isLightTheme;

    public AccountSettingViewModel() : base(true)
    {
        AvailableColors = new(theme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x)));

        foreach (var availableColor in AvailableColors)
        {
            if (availableColor.Value == theme.ActiveColorTheme)
            {
                availableColor.IsSelect = true;
            }
        }

        isLightTheme = theme.ActiveBaseTheme == ThemeVariant.Light;
        ChangePasswordCommand = CreateCommandFromTask(TaskWork.Create(ChangePasswordAsync).RunAsync);
        SwitchToColorThemeCommand =
            CreateCommandFromTask<Selected<SukiColorTheme>>(
                TaskWork.Create<Selected<SukiColorTheme>>(SwitchToColorTheme).RunAsync
            );

        this.WhenAnyValue(x => x.IsLightTheme)
            .Subscribe(x => theme.ChangeBaseTheme(x ? ThemeVariant.Light : ThemeVariant.Dark));
    }

    public override string ViewId => TypeCache<AccountSettingViewModel>.Type.Name;
    public ICommand ChangePasswordCommand { get; }
    public ICommand SwitchToColorThemeCommand { get; }
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    public AvaloniaList<Selected<SukiColorTheme>> AvailableColors { get; }

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

    public bool IsLightTheme
    {
        get => isLightTheme;
        set => this.RaiseAndSetIfChanged(ref isLightTheme, value);
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

    public Task SwitchToColorTheme(Selected<SukiColorTheme> colorTheme, CancellationToken cancellationToken)
    {
        theme.ChangeColorTheme(colorTheme.Value);
        colorTheme.IsSelect = true;

        return Task.CompletedTask;
    }
}