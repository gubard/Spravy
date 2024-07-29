namespace Spravy.Ui.Features.Authentication.Commands;

public class VerificationCodeCommands
{
    private readonly IAuthenticationService authenticationService;
    private readonly IDialogViewer dialogViewer;
    private readonly INavigator navigator;

    public VerificationCodeCommands(
        IAuthenticationService authenticationService,
        SpravyCommandService spravyCommandService,
        IErrorHandler errorHandler,
        IDialogViewer dialogViewer,
        INavigator navigator,
        ITaskProgressService taskProgressService
    )
    {
        this.authenticationService = authenticationService;
        this.dialogViewer = dialogViewer;
        this.navigator = navigator;
        Initialized = SpravyCommand.Create<VerificationCodeViewModel>(
            InitializedAsync,
            errorHandler,
            taskProgressService
        );
        SendNewVerificationCode = spravyCommandService.SendNewVerificationCode;
        UpdateEmail = SpravyCommand.Create<IVerificationEmail>(
            UpdateEmailAsync,
            errorHandler,
            taskProgressService
        );
        Back = spravyCommandService.Back;
        VerificationEmail = SpravyCommand.Create<IVerificationEmail>(
            VerificationEmailAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand Initialized { get; }
    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand UpdateEmail { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand VerificationEmail { get; }

    private ConfiguredValueTaskAwaitable<Result> VerificationEmailAsync(
        IVerificationEmail verificationEmail,
        CancellationToken ct
    )
    {
        switch (verificationEmail.IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService
                    .VerifiedEmailByEmailAsync(
                        verificationEmail.Identifier,
                        verificationEmail.VerificationCode.ToUpperInvariant(),
                        ct
                    )
                    .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct);
            case UserIdentifierType.Login:
                return authenticationService
                    .VerifiedEmailByLoginAsync(
                        verificationEmail.Identifier,
                        verificationEmail.VerificationCode.ToUpperInvariant(),
                        ct
                    )
                    .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct);
            default:
                return new Result(
                    new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType)
                )
                    .ToValueTaskResult()
                    .ConfigureAwait(false);
        }
    }

    private ConfiguredValueTaskAwaitable<Result> UpdateEmailAsync(
        IVerificationEmail verificationEmail,
        CancellationToken ct
    )
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(
            newEmail =>
                dialogViewer
                    .CloseInputDialogAsync(ct)
                    .IfSuccessAsync(
                        () =>
                        {
                            switch (verificationEmail.IdentifierType)
                            {
                                case UserIdentifierType.Email:
                                    return authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
                                        verificationEmail.Identifier,
                                        newEmail,
                                        ct
                                    );
                                case UserIdentifierType.Login:
                                    return authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
                                        verificationEmail.Identifier,
                                        newEmail,
                                        ct
                                    );
                                default:
                                    return new Result(
                                        new UserIdentifierTypeOutOfRangeError(
                                            verificationEmail.IdentifierType
                                        )
                                    )
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false);
                            }
                        },
                        ct
                    ),
            ActionHelper<TextViewModel>.Empty,
            _ => Result.AwaitableSuccess,
            ct
        );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        VerificationCodeViewModel viewModel,
        CancellationToken ct
    )
    {
        switch (viewModel.IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService.UpdateVerificationCodeByEmailAsync(
                    viewModel.Identifier,
                    ct
                );
            case UserIdentifierType.Login:
                return authenticationService.UpdateVerificationCodeByLoginAsync(
                    viewModel.Identifier,
                    ct
                );
            default:
                return new Result(new UserIdentifierTypeOutOfRangeError(viewModel.IdentifierType))
                    .ToValueTaskResult()
                    .ConfigureAwait(false);
        }
    }
}
