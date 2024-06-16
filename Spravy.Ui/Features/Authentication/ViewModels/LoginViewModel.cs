namespace Spravy.Ui.Features.Authentication.ViewModels;

public class LoginViewModel : NavigatableViewModelBase, INotifyDataErrorInfo
{
    private readonly IObjectStorage objectStorage;
    private readonly IPropertyValidator propertyValidator;
    
    private bool loginChanged;
    private bool passwordChanged;
    
    public LoginViewModel(
        LoginCommands commands,
        IObjectStorage objectStorage,
        IPropertyValidator propertyValidator,
        AccountNotify account
    ) : base(true)
    {
        Commands = commands;
        this.objectStorage = objectStorage;
        this.propertyValidator = propertyValidator;
        Account = account;
        
        this.WhenAnyValue(x => x.Login)
           .Skip(1)
           .Subscribe(_ =>
            {
                loginChanged = true;
                this.RaisePropertyChanged(nameof(HasErrors));
            });
        
        this.WhenAnyValue(x => x.Password)
           .Skip(1)
           .Subscribe(_ =>
            {
                passwordChanged = true;
                this.RaisePropertyChanged(nameof(HasErrors));
            });
    }
    
    public AccountNotify Account { get; }
    public LoginCommands Commands { get; }
    
    [Reactive]
    public bool IsBusy { get; set; }
    
    [Reactive]
    public string Login { get; set; } = string.Empty;
    
    [Reactive]
    public string Password { get; set; } = string.Empty;
    
    public override string ViewId
    {
        get => TypeCache<LoginViewModel>.Type.Name;
    }
    
    [Reactive]
    public bool IsRememberMe { get; set; }
    
#pragma warning disable CS0067
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
#pragma warning restore CS0067
    
    public IEnumerable GetErrors(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Login):
            {
                if (loginChanged)
                {
                    var valid = propertyValidator.ValidLogin(Login, nameof(Login));
                    var validLength = propertyValidator.ValidLength(Login, 4, 512, nameof(Login));
                    
                    if (valid is not null)
                    {
                        yield return valid;
                    }
                    
                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                }
                
                break;
            }
            case nameof(Password):
            {
                if (passwordChanged)
                {
                    var valid = propertyValidator.ValidPassword(Password, nameof(Password));
                    var validLength = propertyValidator.ValidLength(Password, 8, 512, nameof(Password));
                    
                    if (valid is not null)
                    {
                        yield return valid;
                    }
                    
                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                }
                
                break;
            }
        }
    }
    
    public bool HasErrors
    {
        get
        {
            if (propertyValidator is null)
            {
                return true;
            }
            
            var hasError = propertyValidator.ValidLogin(Login, nameof(Login)) is not null
             || propertyValidator.ValidLength(Login, 4, 512, nameof(Login)) is not null
             || propertyValidator.ValidPassword(Password, nameof(Password)) is not null
             || propertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null;
            
            return hasError;
        }
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<LoginViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                Login = s.Login;
                
                return Result.Success;
            }), cancellationToken);
    }
}