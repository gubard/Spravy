namespace Spravy.Ui.ViewModels;

public class SettingViewModel : NavigatableViewModelBase
{
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly SukiTheme theme = SukiTheme.GetInstance();
    
    public SettingViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        AvailableColors = new(theme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x)));
        
        foreach (var availableColor in AvailableColors)
        {
            if (availableColor.Value == theme.ActiveColorTheme)
            {
                availableColor.IsSelect = true;
            }
        }
        
        IsLightTheme = theme.ActiveBaseTheme == ThemeVariant.Light;
    }
    
    public override string ViewId
    {
        get => TypeCache<SettingViewModel>.Type.Name;
    }
    
    public ICommand InitializedCommand { get; }
    
    [Inject]
    public required ISpravyNotificationManager SpravyNotificationManager { get; init; }
    
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
    public ICommand ChangePasswordCommand { get; protected set; }
    
    [Reactive]
    public ICommand DeleteAccountCommand { get; protected set; }
    
    [Reactive]
    public ICommand SwitchToColorThemeCommand { get; protected set; }
    
    [Reactive]
    public ICommand SaveSettingsCommand { get; protected set; }
    
    [Reactive]
    public bool IsBusy { get; set; }
    
    [Reactive]
    public bool IsLightTheme { get; set; }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        ChangePasswordCommand = CreateCommandFromTask(TaskWork.Create(ChangePasswordAsync).RunAsync);
        SaveSettingsCommand = CreateCommandFromTask(TaskWork.Create(SaveSettingsAsync).RunAsync);
        DeleteAccountCommand = CreateCommandFromTask(TaskWork.Create(DeleteAccountAsync).RunAsync);
        
        SwitchToColorThemeCommand =
            CreateCommandFromTask<Selected<SukiColorTheme>>(TaskWork
               .Create<Selected<SukiColorTheme>>(SwitchToColorTheme)
               .RunAsync);
        
        this.WhenAnyValue(x => x.IsLightTheme)
           .Subscribe(x => theme.ChangeBaseTheme(x ? ThemeVariant.Light : ThemeVariant.Dark));
        
        return Result.AwaitableFalse;
    }
    
    private ConfiguredValueTaskAwaitable<Result> SaveSettingsAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(TypeCache<SettingModel>.Type.Name, new SettingModel
            {
                BaseTheme = IsLightTheme ? "Light" : "Dark",
                ColorTheme = AvailableColors.Single(x => x.IsSelect).Value.DisplayName,
            })
           .IfSuccessAsync(
                () => SpravyNotificationManager.ShowAsync(new TextLocalization("SettingView.SaveSetting"),
                    cancellationToken), cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<DeleteAccountViewModel>(vm =>
        {
            vm.Identifier = AccountNotify.Login;
            vm.IdentifierType = UserIdentifierType.Login;
        }, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> ChangePasswordAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
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
        return Result.AwaitableFalse;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
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
                }).ToValueTask().ConfigureAwait(false),
                cancellationToken);
    }
}