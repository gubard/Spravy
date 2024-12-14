namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class CreateUserViewModel : NavigatableViewModelBase, INotifyDataErrorInfo
{
    private readonly IPropertyValidator propertyValidator;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string repeatPassword = string.Empty;

    public CreateUserViewModel(IPropertyValidator propertyValidator) : base(true)
    {
        this.propertyValidator = propertyValidator;
    }

    public override string ViewId => TypeCache<CreateUserViewModel>.Name;

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
             || propertyValidator.ValidEquals(
                    Password,
                    RepeatPassword,
                    nameof(Password),
                    nameof(RepeatPassword)
                ) is not null;

            return hasError;
        }
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Login):
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

                break;
            }
            case nameof(Email):
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

                break;
            }
            case nameof(Password):
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

                break;
            }
            case nameof(RepeatPassword):
            {
                var valid = propertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword));
                var validLength = propertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword));

                var validEquals = propertyValidator.ValidEquals(
                    Password,
                    RepeatPassword,
                    nameof(Password),
                    nameof(RepeatPassword)
                );

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

                break;
            }
        }
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}