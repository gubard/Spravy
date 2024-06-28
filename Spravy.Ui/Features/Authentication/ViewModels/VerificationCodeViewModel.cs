namespace Spravy.Ui.Features.Authentication.ViewModels;

public class VerificationCodeViewModel : NavigatableViewModelBase, IVerificationEmail
{
    public VerificationCodeViewModel(VerificationCodeCommands commands)
        : base(true)
    {
        Commands = commands;
    }

    public VerificationCodeCommands Commands { get; }

    public override string ViewId
    {
        get => TypeCache<VerificationCodeViewModel>.Type.Name;
    }

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
