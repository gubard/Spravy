namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class LoginViewModel : NavigatableViewModelBase, INotifyDataErrorInfo
{
    private readonly IObjectStorage objectStorage;
    private readonly IPropertyValidator propertyValidator;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string login = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isRememberMe;

    public LoginViewModel(IObjectStorage objectStorage, IPropertyValidator propertyValidator)
        : base(true)
    {
        this.objectStorage = objectStorage;
        this.propertyValidator = propertyValidator;
    }

    public override string ViewId
    {
        get => TypeCache<LoginViewModel>.Type.Name;
    }

#pragma warning disable CS0067
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
#pragma warning restore CS0067

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
        }
    }

    public bool HasErrors
    {
        get
        {
            var hasError =
                propertyValidator.ValidLogin(Login, nameof(Login)) is not null
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

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this), ct);
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return setting
            .CastObject<LoginViewModelSetting>()
            .IfSuccess(s =>
                this.PostUiBackground(
                    () =>
                    {
                        Login = s.Login;

                        return Result.Success;
                    },
                    ct
                )
            )
            .ToValueTaskResult()
            .ConfigureAwait(false);
    }
}
