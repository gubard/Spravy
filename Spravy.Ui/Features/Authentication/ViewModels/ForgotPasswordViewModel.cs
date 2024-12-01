namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    [ObservableProperty]
    private string emailOrLogin;

    [ObservableProperty]
    private UserIdentifierType identifierType;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string newRepeatPassword = string.Empty;

    [ObservableProperty]
    private string verificationCode = string.Empty;

    public ForgotPasswordViewModel(string emailOrLogin, UserIdentifierType identifierType) : base(true)
    {
        this.emailOrLogin = emailOrLogin;
        this.identifierType = identifierType;
    }

    public override string ViewId => TypeCache<ForgotPasswordViewModel>.Type.Name;

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