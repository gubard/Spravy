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
        Initialized = SpravyCommand.Create<VerificationCodeViewModel>(InitializedAsync, errorHandler, taskProgressService);
        SendNewVerificationCode = spravyCommandService.SendNewVerificationCode;
        UpdateEmail = SpravyCommand.Create<IVerificationEmail>(UpdateEmailAsync, errorHandler, taskProgressService);
        Back = spravyCommandService.Back;
        VerificationEmail = SpravyCommand.Create<IVerificationEmail>(VerificationEmailAsync, errorHandler, taskProgressService);
    }
    
    public SpravyCommand Initialized { get; }
    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand UpdateEmail { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand VerificationEmail { get; }
    
    private ConfiguredValueTaskAwaitable<Result> VerificationEmailAsync(
        IVerificationEmail verificationEmail,
        CancellationToken cancellationToken
    )
    {
        switch (verificationEmail.IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService
                   .VerifiedEmailByEmailAsync(verificationEmail.Identifier,
                        verificationEmail.VerificationCode.ToUpperInvariant(), cancellationToken)
                   .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(cancellationToken),
                        cancellationToken);
            case UserIdentifierType.Login:
                return authenticationService
                   .VerifiedEmailByLoginAsync(verificationEmail.Identifier,
                        verificationEmail.VerificationCode.ToUpperInvariant(), cancellationToken)
                   .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(cancellationToken),
                        cancellationToken);
            default:
                return new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                   .ToValueTaskResult()
                   .ConfigureAwait(false);
        }
    }
    
    private ConfiguredValueTaskAwaitable<Result> UpdateEmailAsync(
        IVerificationEmail verificationEmail,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(newEmail => dialogViewer
           .CloseInputDialogAsync(cancellationToken)
           .IfSuccessAsync(() =>
            {
                switch (verificationEmail.IdentifierType)
                {
                    case UserIdentifierType.Email:
                        return authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
                            verificationEmail.Identifier, newEmail, cancellationToken);
                    case UserIdentifierType.Login:
                        return authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
                            verificationEmail.Identifier, newEmail, cancellationToken);
                    default:
                        return new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                           .ToValueTaskResult()
                           .ConfigureAwait(false);
                }
            }, cancellationToken), ActionHelper<TextViewModel>.Empty, cancellationToken);
    }
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        VerificationCodeViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        switch (viewModel.IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService.UpdateVerificationCodeByEmailAsync(viewModel.Identifier,
                    cancellationToken);
            case UserIdentifierType.Login:
                return authenticationService.UpdateVerificationCodeByLoginAsync(viewModel.Identifier,
                    cancellationToken);
            default:
                return new Result(new UserIdentifierTypeOutOfRangeError(viewModel.IdentifierType)).ToValueTaskResult()
                   .ConfigureAwait(false);
        }
    }
}