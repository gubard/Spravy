using CreateUserView = Spravy.Ui.Features.Authentication.Views.CreateUserView;

namespace Spravy.Ui.Features.Authentication.ViewModels;

public class CreateUserViewModel : NavigatableViewModelBase, ICreateUserProperties, INotifyDataErrorInfo
{
    private bool emailChanged;
    private bool loginChanged;
    private bool passwordChanged;
    private bool repeatPasswordChanged;

    public CreateUserViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required IPropertyValidator PropertyValidator { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public override string ViewId
    {
        get => TypeCache<CreateUserViewModel>.Type.Name;
    }

    [Reactive]
    public ICommand EnterCommand { get; protected set; }

    [Reactive]
    public ICommand CreateUserCommand { get; protected set; }

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

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

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
             || PropertyValidator.ValidEmail(Email, nameof(Email)) is not null
             || PropertyValidator.ValidLength(Email, 6, 50, nameof(Email)) is not null
             || PropertyValidator.ValidPassword(Password, nameof(Password)) is not null
             || PropertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null
             || PropertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword)) is not null
             || PropertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword)) is not null
             || PropertyValidator.ValidEquals(Password, RepeatPassword, nameof(Password), nameof(RepeatPassword)) is not
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
            case nameof(Email):
            {
                if (emailChanged)
                {
                    var valid = PropertyValidator.ValidEmail(Email, nameof(Email));
                    var validLength = PropertyValidator.ValidLength(Email, 6, 50, nameof(Email));

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
            case nameof(RepeatPassword):
            {
                if (repeatPasswordChanged)
                {
                    var valid = PropertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword));
                    var validLength = PropertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword));

                    var validEquals = PropertyValidator.ValidEquals(Password, RepeatPassword, nameof(Password),
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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        var createUserWork = TaskWork.Create(CreateUserAsync);
        EnterCommand = CreateCommandFromTask<CreateUserView>(TaskWork.Create<CreateUserView>(EnterAsync).RunAsync);
        
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

        CreateUserCommand = CreateCommandFromTask(
            createUserWork.RunAsync, this.WhenAnyValue(x => x.HasErrors).Select(x => !x));

        return Result.AwaitableSuccess;
    }

    private ConfiguredValueTaskAwaitable<Result> EnterAsync(CreateUserView view, CancellationToken cancellationToken)
    {
        var emailTextBox = view.FindControl<TextBox>(CreateUserView.EmailTextBoxName);

        if (emailTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        var loginTextBox = view.FindControl<TextBox>(CreateUserView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (emailTextBox.IsFocused)
        {
            loginTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        var passwordTextBox = view.FindControl<TextBox>(CreateUserView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        var repeatPasswordTextBox = view.FindControl<TextBox>(CreateUserView.RepeatPasswordTextBoxName);

        if (repeatPasswordTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (passwordTextBox.IsFocused)
        {
            repeatPasswordTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        if (HasErrors)
        {
            return Result.AwaitableSuccess;
        }

        return CreateUserAsync(cancellationToken);
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

    private ConfiguredValueTaskAwaitable<Result> CreateUserAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                 IsBusy = true;
                 
                 return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() =>
                {
                    var options = Mapper.Map<CreateUserOptions>(this);

                    return AuthenticationService.CreateUserAsync(options, cancellationToken)
                       .IfSuccessAsync(() => Navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                        {
                            vm.Identifier = Email;
                            vm.IdentifierType = UserIdentifierType.Email;
                        }, cancellationToken), cancellationToken);
                }, () => this.InvokeUiBackgroundAsync(() =>
                {
                     IsBusy = false;
                     
                     return Result.Success;
                }).ToValueTask().ConfigureAwait(false),
                cancellationToken);
    }
}