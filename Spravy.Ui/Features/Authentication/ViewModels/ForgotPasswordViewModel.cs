namespace Spravy.Ui.Features.Authentication.ViewModels;

public class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    private readonly IAuthenticationService authenticationService;
    private readonly INavigator navigator;
    
    public ForgotPasswordViewModel(
        IAuthenticationService authenticationService,
        IErrorHandler errorHandler,
        INavigator navigator
    ) : base(true)
    {
        this.authenticationService = authenticationService;
        this.navigator = navigator;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler);
        ForgotPasswordCommand = SpravyCommand.Create(ForgotPasswordAsync, errorHandler);
    }
    
    public SpravyCommand InitializedCommand { get; }
    public SpravyCommand ForgotPasswordCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<ForgotPasswordViewModel>.Type.Name;
    }
    
    [Reactive]
    public string NewPassword { get; set; } = string.Empty;
    
    [Reactive]
    public string NewRepeatPassword { get; set; } = string.Empty;
    
    [Reactive]
    public string Identifier { get; set; } = string.Empty;
    
    [Reactive]
    public UserIdentifierType IdentifierType { get; set; }
    
    [Reactive]
    public string VerificationCode { get; set; } = string.Empty;
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService.UpdateVerificationCodeByEmailAsync(Identifier, cancellationToken);
            case UserIdentifierType.Login:
                return authenticationService.UpdateVerificationCodeByLoginAsync(Identifier, cancellationToken);
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess
           .IfSuccessAsync(() =>
            {
                switch (IdentifierType)
                {
                    case UserIdentifierType.Email:
                        return authenticationService.UpdatePasswordByEmailAsync(Identifier,
                            VerificationCode.ToUpperInvariant(), NewPassword, cancellationToken);
                    case UserIdentifierType.Login:
                        return authenticationService.UpdatePasswordByLoginAsync(Identifier,
                            VerificationCode.ToUpperInvariant(), NewPassword, cancellationToken);
                    default: throw new ArgumentOutOfRangeException();
                }
            }, cancellationToken)
           .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(cancellationToken), cancellationToken);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return Result.AwaitableSuccess;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableSuccess;
    }
}