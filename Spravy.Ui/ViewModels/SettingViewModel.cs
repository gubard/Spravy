using Spravy.Ui.Mappers;
using AppConst = Spravy.Domain.Helpers.AppConst;

namespace Spravy.Ui.ViewModels;

public partial class SettingViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly Application application;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private ThemeType selectedTheme;

    [ObservableProperty]
    private bool isBusy;

    public SettingViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        AccountNotify accountNotify,
        ITaskProgressService taskProgressService,
        Application application,
        IObjectStorage objectStorage,
        IViewFactory viewFactory
    )
        : base(true)
    {
        this.application = application;
        this.navigator = navigator;
        AccountNotify = accountNotify;
        this.objectStorage = objectStorage;
        this.viewFactory = viewFactory;
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

        PropertyChanged += OnPropertyChanged;
    }

    public AccountNotify AccountNotify { get; }
    public SpravyCommand ChangePasswordCommand { get; }
    public SpravyCommand DeleteAccountCommand { get; }
    public SpravyCommand SaveCommand { get; }

    public override string ViewId
    {
        get => TypeCache<SettingViewModel>.Type.Name;
    }

    public string Version
    {
        get => $"{AppConst.Version}({AppConst.Version.Code})";
    }

    private Cvtar DeleteAccountAsync(CancellationToken ct)
    {
        return navigator.NavigateToAsync(
            viewFactory.CreateDeleteAccountViewModel(AccountNotify.Login, UserIdentifierType.Login),
            ct
        );
    }

    private Cvtar ChangePasswordAsync(CancellationToken ct)
    {
        return navigator.NavigateToAsync(
            viewFactory.CreateForgotPasswordViewModel(
                AccountNotify.Login,
                UserIdentifierType.Login
            ),
            ct
        );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<Setting.Setting>(ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
                        () =>
                        {
                            SelectedTheme = setting.Theme;

                            return Result.Success;
                        },
                        ct
                    ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new Setting.Setting(this), ct);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTheme))
        {
            application.RequestedThemeVariant = SelectedTheme.ToThemeVariant();
        }
    }
}
