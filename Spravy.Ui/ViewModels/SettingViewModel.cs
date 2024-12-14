using Spravy.Ui.Setting;
using AppConst = Spravy.Domain.Helpers.AppConst;

namespace Spravy.Ui.ViewModels;

public partial class SettingViewModel : NavigatableViewModelBase
{
    private readonly Application application;
    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly IViewFactory viewFactory;
    public string[] FavoriteIcons = [];

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string language;

    [ObservableProperty]
    private ThemeType selectedTheme;

    public SettingViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        AccountNotify accountNotify,
        ITaskProgressService taskProgressService,
        Application application,
        IObjectStorage objectStorage,
        IViewFactory viewFactory
    ) : base(true)
    {
        this.application = application;
        this.navigator = navigator;
        AccountNotify = accountNotify;
        this.objectStorage = objectStorage;
        this.viewFactory = viewFactory;
        SaveCommand = SpravyCommand.Create(SaveStateAsync, errorHandler, taskProgressService);
        ChangePasswordCommand = SpravyCommand.Create(ChangePasswordAsync, errorHandler, taskProgressService);
        DeleteAccountCommand = SpravyCommand.Create(DeleteAccountAsync, errorHandler, taskProgressService);
        PropertyChanged += OnPropertyChanged;

        Languages =
        [
            "en-US",
            "uk-UA",
        ];

        language = Languages[0];
    }

    public AccountNotify AccountNotify { get; }
    public SpravyCommand ChangePasswordCommand { get; }
    public SpravyCommand DeleteAccountCommand { get; }
    public SpravyCommand SaveCommand { get; }
    public AvaloniaList<string> Languages { get; }
    public override string ViewId => TypeCache<SettingViewModel>.Name;
    public string Version => AppConst.VersionString;

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
            viewFactory.CreateForgotPasswordViewModel(AccountNotify.Login, UserIdentifierType.Login),
            ct
        );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<AppSetting>(ViewId, ct)
           .IfSuccessAsync(
                setting => this.PostUiBackground(
                    () =>
                    {
                        SelectedTheme = setting.Theme;
                        Language = setting.Language;
                        FavoriteIcons = setting.FavoriteIcons;

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AppSetting(this), ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTheme))
        {
            application.RequestedThemeVariant = SelectedTheme.ToThemeVariant();
        }
        else if (e.PropertyName == nameof(Language))
        {
            var lang = application.GetLang(Language);
            application.Resources.MergedDictionaries.Remove(lang);
            application.Resources.MergedDictionaries.Add(lang);
        }
    }
}