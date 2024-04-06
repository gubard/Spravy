using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Styling;
using Ninject;
using ReactiveUI;
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
using SukiUI;
using SukiUI.Models;

namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    private readonly SukiTheme theme = SukiTheme.GetInstance();
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public SettingViewModel() : base(true)
    {
        AvailableColors = new(theme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x)));

        foreach (var availableColor in AvailableColors)
        {
            if (availableColor.Value == theme.ActiveColorTheme)
            {
                availableColor.IsSelect = true;
            }
        }

        IsLightTheme = theme.ActiveBaseTheme == ThemeVariant.Light;
        ChangePasswordCommand = CreateCommandFromTask(TaskWork.Create(ChangePasswordAsync).RunAsync);
        SaveSettingsCommand = CreateCommandFromTask(TaskWork.Create(SaveSettingsAsync).RunAsync);
        DeleteAccountCommand = CreateCommandFromTask(TaskWork.Create(DeleteAccountAsync).RunAsync);

        SwitchToColorThemeCommand =
            CreateCommandFromTask<Selected<SukiColorTheme>>(
                TaskWork.Create<Selected<SukiColorTheme>>(SwitchToColorTheme).RunAsync
            );

        this.WhenAnyValue(x => x.IsLightTheme)
            .Subscribe(x => theme.ChangeBaseTheme(x ? ThemeVariant.Light : ThemeVariant.Dark));
    }

    public override string ViewId => TypeCache<SettingViewModel>.Type.Name;
    public ICommand ChangePasswordCommand { get; }
    public ICommand DeleteAccountCommand { get; }
    public ICommand SwitchToColorThemeCommand { get; }
    public ICommand SaveSettingsCommand { get; }
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    public AvaloniaList<Selected<SukiColorTheme>> AvailableColors { get; }

    [Inject]
    public required AccountNotify AccountNotify { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

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

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool IsLightTheme { get; set; }

    private ConfiguredValueTaskAwaitable<Result> SaveSettingsAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .IfSuccessTryFinallyAsync(
                () => ObjectStorage.SaveObjectAsync(
                    TypeCache<SettingModel>.Type.Name,
                    new SettingModel
                    {
                        BaseTheme = IsLightTheme ? "Light" : "Dark",
                        ColorTheme = AvailableColors.Single(x => x.IsSelect).Value.DisplayName,
                    }
                ),
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ToValueTask()
                    .ConfigureAwait(false)
            );
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<DeleteAccountViewModel>(
            vm =>
            {
                vm.Identifier = AccountNotify.Login;
                vm.IdentifierType = UserIdentifierType.Login;
            },
            cancellationToken
        );
    }

    private ConfiguredValueTaskAwaitable<Result> ChangePasswordAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<ForgotPasswordViewModel>(
            vm =>
            {
                vm.Identifier = AccountNotify.Login;
                vm.IdentifierType = UserIdentifierType.Login;
            },
            cancellationToken
        );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync()
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(object setting)
    {
        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result> SwitchToColorTheme(
        Selected<SukiColorTheme> colorTheme,
        CancellationToken cancellationToken
    )
    {
        theme.ChangeColorTheme(colorTheme.Value);
        colorTheme.IsSelect = true;

        return Result.AwaitableFalse;
    }
}