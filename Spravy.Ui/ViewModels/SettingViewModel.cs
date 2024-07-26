namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly Application application;

    public SettingViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        AccountNotify accountNotify,
        ITaskProgressService taskProgressService,
        Application application
    )
        : base(true)
    {
        this.navigator = navigator;
        AccountNotify = accountNotify;
        this.application = application;
        ChangePasswordCommand = SpravyCommand.Create(
            ChangePasswordAsync,
            errorHandler,
            taskProgressService
        );
        DeleteAccountCommand = SpravyCommand.Create(
            DeleteAccountAsync,
            errorHandler,
            taskProgressService
        );

        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(x =>
                application.RequestedThemeVariant = x switch
                {
                    ThemeType.Default => ThemeVariant.Default,
                    ThemeType.Light => ThemeVariant.Light,
                    ThemeType.Dark => ThemeVariant.Dark,
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                }
            );
    }

    public override string ViewId
    {
        get => TypeCache<SettingViewModel>.Type.Name;
    }

    public string Version
    {
        get
        {
            var versionString =
                Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

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

    public AccountNotify AccountNotify { get; }
    public SpravyCommand ChangePasswordCommand { get; }
    public SpravyCommand DeleteAccountCommand { get; }

    [Reactive]
    public ThemeType SelectedTheme { get; set; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool IsLightTheme { get; set; }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken ct)
    {
        return navigator.NavigateToAsync<DeleteAccountViewModel>(
            vm =>
            {
                vm.Identifier = AccountNotify.Login;
                vm.IdentifierType = UserIdentifierType.Login;
            },
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> ChangePasswordAsync(CancellationToken ct)
    {
        return navigator.NavigateToAsync<ForgotPasswordViewModel>(
            vm =>
            {
                vm.Identifier = AccountNotify.Login;
                vm.IdentifierType = UserIdentifierType.Login;
            },
            ct
        );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }
}
