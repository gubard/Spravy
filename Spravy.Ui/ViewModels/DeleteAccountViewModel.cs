namespace Spravy.Ui.ViewModels;

public partial class DeleteAccountViewModel : NavigatableViewModelBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly INavigator navigator;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private string emailOrLogin;

    [ObservableProperty]
    private UserIdentifierType identifierType;

    [ObservableProperty]
    private string verificationCode = string.Empty;

    public DeleteAccountViewModel(
        string emailOrLogin,
        UserIdentifierType identifierType,
        IErrorHandler errorHandler,
        INavigator navigator,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory
    ) : base(true)
    {
        this.emailOrLogin = emailOrLogin;
        this.identifierType = identifierType;
        this.navigator = navigator;
        this.authenticationService = authenticationService;
        this.viewFactory = viewFactory;
        DeleteAccountCommand = SpravyCommand.Create(DeleteAccountAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand DeleteAccountCommand { get; }

    public override string ViewId => TypeCache<DeleteAccountViewModel>.Name;

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
        return IdentifierType switch
        {
            UserIdentifierType.Email => authenticationService.UpdateVerificationCodeByEmailAsync(EmailOrLogin, ct),
            UserIdentifierType.Login => authenticationService.UpdateVerificationCodeByLoginAsync(EmailOrLogin, ct),
            _ => new Result(new UserIdentifierTypeOutOfRangeError(IdentifierType)).ToValueTaskResult()
               .ConfigureAwait(false),
        };
    }

    private Cvtar DeleteAccountAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess
           .IfSuccessAsync(
                () => IdentifierType switch
                {
                    UserIdentifierType.Email => authenticationService.DeleteUserByEmailAsync(
                        EmailOrLogin,
                        VerificationCode.ToUpperInvariant(),
                        ct
                    ),
                    UserIdentifierType.Login => authenticationService.DeleteUserByEmailAsync(
                        EmailOrLogin,
                        VerificationCode.ToUpperInvariant(),
                        ct
                    ),
                    _ => new Result(new UserIdentifierTypeOutOfRangeError(IdentifierType)).ToValueTaskResult()
                       .ConfigureAwait(false),
                },
                ct
            )
           .IfSuccessAsync(() => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct), ct);
    }
}