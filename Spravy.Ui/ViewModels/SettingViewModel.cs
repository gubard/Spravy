namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly SukiTheme theme = SukiTheme.GetInstance();
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private readonly IObjectStorage objectStorage;

    public SettingViewModel(
        ISpravyNotificationManager spravyNotificationManager,
        IErrorHandler errorHandler,
        INavigator navigator,
        AccountNotify accountNotify,
        IObjectStorage objectStorage,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        this.spravyNotificationManager = spravyNotificationManager;
        this.navigator = navigator;
        AccountNotify = accountNotify;
        this.objectStorage = objectStorage;
        AvailableColors = new(theme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x)));

        foreach (var availableColor in AvailableColors)
        {
            if (availableColor.Value == theme.ActiveColorTheme)
            {
                availableColor.IsSelect = true;
            }
        }

        IsLightTheme = theme.ActiveBaseTheme == ThemeVariant.Light;
        ChangePasswordCommand = SpravyCommand.Create(ChangePasswordAsync, errorHandler, taskProgressService);
        SaveSettingsCommand = SpravyCommand.Create(SaveSettingsAsync, errorHandler, taskProgressService);
        DeleteAccountCommand = SpravyCommand.Create(DeleteAccountAsync, errorHandler, taskProgressService);

        SwitchToColorThemeCommand =
            SpravyCommand.Create<Selected<SukiColorTheme>>(SwitchToColorTheme, errorHandler, taskProgressService);

        this.WhenAnyValue(x => x.IsLightTheme)
           .Subscribe(x => theme.ChangeBaseTheme(x ? ThemeVariant.Light : ThemeVariant.Dark));
    }

    public override string ViewId
    {
        get => TypeCache<SettingViewModel>.Type.Name;
    }

    public string Version
    {
        get
        {
            var versionString = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

            if (versionString.IsNullOrWhiteSpace())
            {
                return "1.0.0.0(0)";
            }

            if (!SpravyVersion.TryParse(versionString, out var version))
            {
                return "1.0.0.0(0)";
            }

            return $"{version}({version.Code})";
        }
    }

    public AvaloniaList<Selected<SukiColorTheme>> AvailableColors { get; }
    public AccountNotify AccountNotify { get; }
    public SpravyCommand ChangePasswordCommand { get; }
    public SpravyCommand DeleteAccountCommand { get; }
    public SpravyCommand SwitchToColorThemeCommand { get; }
    public SpravyCommand SaveSettingsCommand { get; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool IsLightTheme { get; set; }

    private ConfiguredValueTaskAwaitable<Result> SaveSettingsAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(TypeCache<SettingModel>.Type.Name, new SettingModel
            {
                BaseTheme = IsLightTheme ? "Light" : "Dark",
                ColorTheme = AvailableColors.Single(x => x.IsSelect).Value.DisplayName,
            })
           .IfSuccessAsync(
                () => spravyNotificationManager.ShowAsync(new TextLocalization("SettingView.SaveSetting"),
                    cancellationToken), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return navigator.NavigateToAsync<DeleteAccountViewModel>(vm =>
        {
            vm.Identifier = AccountNotify.Login;
            vm.IdentifierType = UserIdentifierType.Login;
        }, cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> ChangePasswordAsync(CancellationToken cancellationToken)
    {
        return navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
        {
            vm.Identifier = AccountNotify.Login;
            vm.IdentifierType = UserIdentifierType.Login;
        }, cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableSuccess;
    }

    public ConfiguredValueTaskAwaitable<Result> SwitchToColorTheme(
        Selected<SukiColorTheme> colorTheme,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                IsBusy = true;

                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() => this.InvokeUiBackgroundAsync(() =>
            {
                theme.ChangeColorTheme(colorTheme.Value);
                colorTheme.IsSelect = true;

                return Result.Success;
            }), () => this.InvokeUiBackgroundAsync(() =>
                {
                    IsBusy = false;

                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), cancellationToken);
    }
}