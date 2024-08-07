namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string newRepeatPassword = string.Empty;

    [ObservableProperty]
    private string identifier = string.Empty;

    [ObservableProperty]
    private UserIdentifierType identifierType;

    [ObservableProperty]
    private string verificationCode = string.Empty;

    public ForgotPasswordViewModel()
        : base(true) { }

    public override string ViewId
    {
        get => TypeCache<ForgotPasswordViewModel>.Type.Name;
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
