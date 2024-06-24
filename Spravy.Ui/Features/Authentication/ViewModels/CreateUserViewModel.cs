namespace Spravy.Ui.Features.Authentication.ViewModels;

public class CreateUserViewModel : NavigatableViewModelBase, ICreateUserProperties, INotifyDataErrorInfo
{
    private readonly IPropertyValidator propertyValidator;
    
    private bool emailChanged;
    private bool loginChanged;
    private bool passwordChanged;
    private bool repeatPasswordChanged;
    
    public CreateUserViewModel(IPropertyValidator propertyValidator, CreateUserCommands commands) : base(true)
    {
        this.propertyValidator = propertyValidator;
        Commands = commands;
        
        this.WhenAnyValue(x => x.Email)
           .Skip(1)
           .Subscribe(_ =>
            {
                emailChanged = true;
                this.RaisePropertyChanged(nameof(HasErrors));
            });
        
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
        
        this.WhenAnyValue(x => x.RepeatPassword)
           .Skip(1)
           .Subscribe(_ =>
            {
                repeatPasswordChanged = true;
                this.RaisePropertyChanged(nameof(HasErrors));
            });
    }
    
    public CreateUserCommands Commands { get; }
    
    public override string ViewId
    {
        get => TypeCache<CreateUserViewModel>.Type.Name;
    }
    
    [Reactive]
    public bool IsBusy { get; set; }
    
    [Reactive]
    public string Email { get; set; } = string.Empty;
    
    [Reactive]
    public string Login { get; set; } = string.Empty;
    
    [Reactive]
    public string Password { get; set; } = string.Empty;
    
    [Reactive]
    public string RepeatPassword { get; set; } = string.Empty;
    
#pragma warning disable CS0067
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
#pragma warning restore CS0067
    
    public bool HasErrors
    {
        get
        {
            var hasError = propertyValidator.ValidLogin(Login, nameof(Login)) is not null
             || propertyValidator.ValidLength(Login, 4, 512, nameof(Login)) is not null
             || propertyValidator.ValidEmail(Email, nameof(Email)) is not null
             || propertyValidator.ValidLength(Email, 6, 50, nameof(Email)) is not null
             || propertyValidator.ValidPassword(Password, nameof(Password)) is not null
             || propertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null
             || propertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword)) is not null
             || propertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword)) is not null
             || propertyValidator.ValidEquals(Password, RepeatPassword, nameof(Password), nameof(RepeatPassword)) is not
                    null;
            
            return hasError;
        }
    }
    
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
            case nameof(Email):
            {
                if (emailChanged)
                {
                    var valid = propertyValidator.ValidEmail(Email, nameof(Email));
                    var validLength = propertyValidator.ValidLength(Email, 6, 50, nameof(Email));
                    
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
            case nameof(RepeatPassword):
            {
                if (repeatPasswordChanged)
                {
                    var valid = propertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword));
                    var validLength = propertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword));
                    
                    var validEquals = propertyValidator.ValidEquals(Password, RepeatPassword, nameof(Password),
                        nameof(RepeatPassword));
                    
                    if (valid is not null)
                    {
                        yield return valid;
                    }
                    
                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                    
                    if (validEquals is not null)
                    {
                        yield return validEquals;
                    }
                }
                
                break;
            }
        }
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