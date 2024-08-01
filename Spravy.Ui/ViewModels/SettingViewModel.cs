using Spravy.Ui.Mappers;

namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;

    public SettingViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        AccountNotify accountNotify,
        ITaskProgressService taskProgressService,
        Application application,
        IObjectStorage objectStorage
    )
        : base(true)
    {
        this.navigator = navigator;
        AccountNotify = accountNotify;
        this.objectStorage = objectStorage;
        SaveCommand = SpravyCommand.Create(SaveStateAsync, errorHandler, taskProgressService);

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
            .Subscribe(x => application.RequestedThemeVariant = x.ToThemeVariant());
    }

    public AccountNotify AccountNotify { get; }
    public SpravyCommand ChangePasswordCommand { get; }
    public SpravyCommand DeleteAccountCommand { get; }
    public SpravyCommand SaveCommand { get; }

    [Reactive]
    public ThemeType SelectedTheme { get; set; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool IsLightTheme { get; set; }

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
        return objectStorage.SaveObjectAsync(ViewId, new Setting.Setting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<Setting.Setting>()
            .IfSuccess(s =>
                s.PostUiBackground(
                    () =>
                    {
                        SelectedTheme = s.Theme;

                        return Result.Success;
                    },
                    ct
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }
}
