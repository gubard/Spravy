namespace Spravy.Ui.Features.Authentication.ViewModels;

public class LoginViewModel : NavigatableViewModelBase, ILoginProperties, INotifyDataErrorInfo
{
    private bool loginChanged;
    private bool passwordChanged;

    public LoginViewModel() : base(true)
    {
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
    
    [Inject]
    public required LoginCommands Commands { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IPropertyValidator PropertyValidator { get; init; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool TryAutoLogin { get; set; }

    [Reactive]
    public string Login { get; set; } = string.Empty;

    [Reactive]
    public string Password { get; set; } = string.Empty;

    public override string ViewId
    {
        get => TypeCache<LoginViewModel>.Type.Name;
    }

    [Inject]
    public required AccountNotify Account { get; init; }

    [Reactive]
    public bool IsRememberMe { get; set; }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Login):
            {
                if (loginChanged)
                {
                    var valid = PropertyValidator.ValidLogin(Login, nameof(Login));
                    var validLength = PropertyValidator.ValidLength(Login, 4, 512, nameof(Login));

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
                    var valid = PropertyValidator.ValidPassword(Password, nameof(Password));
                    var validLength = PropertyValidator.ValidLength(Password, 8, 512, nameof(Password));

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
            if (PropertyValidator is null)
            {
                return true;
            }

            var hasError = PropertyValidator.ValidLogin(Login, nameof(Login)) is not null
             || PropertyValidator.ValidLength(Login, 4, 512, nameof(Login)) is not null
             || PropertyValidator.ValidPassword(Password, nameof(Password)) is not null
             || PropertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null;

            return hasError;
        }
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<LoginViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUiBackgroundAsync(() =>
            {
                TryAutoLogin = false;
                Login = s.Login;
                
                return Result.Success;
            }), cancellationToken);
    }
}