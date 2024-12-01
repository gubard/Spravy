namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class VerificationCodeViewModel : NavigatableViewModelBase, IVerificationEmail
{
    [ObservableProperty]
    private string verificationCode = string.Empty;

    public VerificationCodeViewModel(string emailOrLogin, UserIdentifierType identifierType) : base(true)
    {
        EmailOrLogin = emailOrLogin;
        IdentifierType = identifierType;
    }

    public override string ViewId => TypeCache<VerificationCodeViewModel>.Type.Name;

    public string EmailOrLogin { get; }
    public UserIdentifierType IdentifierType { get; }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}