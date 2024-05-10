using Spravy.Ui.Errors;

namespace Spravy.Ui.Features.Authentication.Commands;

public class VerificationCodeCommands
{
    private readonly IAuthenticationService authenticationService;
    private readonly IDialogViewer dialogViewer;
    private readonly INavigator navigator;
    
    public VerificationCodeCommands(
        IAuthenticationService authenticationService,
        IErrorHandler errorHandler,
        IDialogViewer dialogViewer,
        INavigator navigator
    )
    {
        this.authenticationService = authenticationService;
        this.dialogViewer = dialogViewer;
        this.navigator = navigator;
        Initialized = SpravyCommand.Create<VerificationCodeViewModel>(InitializedAsync, errorHandler);
        SendNewVerificationCode = SpravyCommand.CreateSendNewVerificationCode(authenticationService, errorHandler);
        UpdateEmail = SpravyCommand.Create<IVerificationEmail>(UpdateEmailAsync, errorHandler);
        Back = SpravyCommand.CreateBack(navigator, errorHandler);
        VerificationEmail = SpravyCommand.Create<IVerificationEmail>(VerificationEmailAsync, errorHandler);
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
           .IfSuccessAllAsync(cancellationToken, () =>
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
            }), ActionHelper<TextViewModel>.Empty, cancellationToken);
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