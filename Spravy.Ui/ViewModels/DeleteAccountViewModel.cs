namespace Spravy.Ui.ViewModels;

public class DeleteAccountViewModel : NavigatableViewModelBase
{
    public DeleteAccountViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }

    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;

    [Reactive]
    public string Identifier { get; set; } = string.Empty;

    [Reactive]
    public ICommand DeleteAccountCommand { get; protected set; }

    public override string ViewId
    {
        get => TypeCache<DeleteAccountViewModel>.Type.Name;
    }

    public ICommand InitializedCommand { get; }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse;
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableFalse
           .IfSuccessAsync(() =>
            {
                switch (IdentifierType)
                {
                    case UserIdentifierType.Email:
                        return AuthenticationService.DeleteUserByEmailAsync(Identifier,
                            VerificationCode.ToUpperInvariant(),
                            cancellationToken);
                    case UserIdentifierType.Login:
                        return AuthenticationService.DeleteUserByEmailAsync(Identifier,
                            VerificationCode.ToUpperInvariant(),
                            cancellationToken);
                    default: throw new ArgumentOutOfRangeException();
                }
            }, cancellationToken)
           .IfSuccessAsync(() => Navigator.NavigateToAsync<LoginViewModel>(cancellationToken), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        DeleteAccountCommand = CreateCommandFromTask(TaskWork.Create(DeleteAccountAsync).RunAsync);

        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return AuthenticationService.UpdateVerificationCodeByEmailAsync(Identifier, cancellationToken);
            case UserIdentifierType.Login:
                return AuthenticationService.UpdateVerificationCodeByLoginAsync(Identifier, cancellationToken);
            default: throw new ArgumentOutOfRangeException();
        }
    }
}