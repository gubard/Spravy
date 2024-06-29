namespace Spravy.Ui.Features.Authentication.ViewModels;

public class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    public ForgotPasswordViewModel()
        : base(true) { }

    public override string ViewId
    {
        get => TypeCache<ForgotPasswordViewModel>.Type.Name;
    }

    [Reactive]
    public string NewPassword { get; set; } = string.Empty;

    [Reactive]
    public string NewRepeatPassword { get; set; } = string.Empty;

    [Reactive]
    public string Identifier { get; set; } = string.Empty;

    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }

    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;

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
