namespace Spravy.Ui.Features.Authentication.ViewModels;

public partial class VerificationCodeViewModel : NavigatableViewModelBase, IVerificationEmail
{
    [ObservableProperty]
    private string identifier = string.Empty;

    [ObservableProperty]
    private UserIdentifierType identifierType;

    [ObservableProperty]
    private string verificationCode = string.Empty;

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
