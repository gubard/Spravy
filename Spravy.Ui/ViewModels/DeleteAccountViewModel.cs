namespace Spravy.Ui.ViewModels;

public partial class DeleteAccountViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly IAuthenticationService authenticationService;

    [ObservableProperty]
    private UserIdentifierType identifierType;

    [ObservableProperty]
    private string verificationCode = string.Empty;

    [ObservableProperty]
    private string identifier = string.Empty;

    public DeleteAccountViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        this.navigator = navigator;
        this.authenticationService = authenticationService;

        InitializedCommand = SpravyCommand.Create(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );

        DeleteAccountCommand = SpravyCommand.Create(
            DeleteAccountAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand DeleteAccountCommand { get; }
    public SpravyCommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<DeleteAccountViewModel>.Type.Name;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        return Result.AwaitableSuccess;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    private ConfiguredValueTaskAwaitable<Result> DeleteAccountAsync(CancellationToken ct)
    {
        return Result
            .AwaitableSuccess.IfSuccessAsync(
                () =>
                {
                    switch (IdentifierType)
                    {
                        case UserIdentifierType.Email:
                            return authenticationService.DeleteUserByEmailAsync(
                                Identifier,
                                VerificationCode.ToUpperInvariant(),
                                ct
                            );
                        case UserIdentifierType.Login:
                            return authenticationService.DeleteUserByEmailAsync(
                                Identifier,
                                VerificationCode.ToUpperInvariant(),
                                ct
                            );
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                },
                ct
            )
            .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        return IdentifierType switch
        {
            UserIdentifierType.Email
                => authenticationService.UpdateVerificationCodeByEmailAsync(Identifier, ct),
            UserIdentifierType.Login
                => authenticationService.UpdateVerificationCodeByLoginAsync(Identifier, ct),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
