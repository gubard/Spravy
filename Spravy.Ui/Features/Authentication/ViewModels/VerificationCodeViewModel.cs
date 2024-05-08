namespace Spravy.Ui.Features.Authentication.ViewModels;

public class VerificationCodeViewModel : NavigatableViewModelBase, IVerificationEmail
{
    public VerificationCodeViewModel() : base(true)
    {
    }

    public override string ViewId
    {
        get => TypeCache<VerificationCodeViewModel>.Type.Name;
    }

    [Inject]
    public required VerificationCodeCommands Commands { get; init; }

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

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
    }
}