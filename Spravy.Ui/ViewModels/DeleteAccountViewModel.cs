namespace Spravy.Ui.ViewModels;

public class DeleteAccountViewModel : NavigatableViewModelBase
{
    public DeleteAccountViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        DeleteAccountCommand = CreateCommandFromTask(TaskWork.Create(DeleteAccountAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }

    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;

    [Reactive]
    public string Identifier { get; set; } = string.Empty;

    public ICommand DeleteAccountCommand { get; }

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
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess
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
        return IdentifierType switch
        {
            UserIdentifierType.Email => AuthenticationService.UpdateVerificationCodeByEmailAsync(Identifier,
                cancellationToken),
            UserIdentifierType.Login => AuthenticationService.UpdateVerificationCodeByLoginAsync(Identifier,
                cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}