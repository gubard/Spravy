namespace Spravy.Ui.Features.Authentication.ViewModels;

public class ForgotPasswordViewModel : NavigatableViewModelBase, IVerificationEmail
{
    private readonly IAuthenticationService authenticationService;
    private readonly INavigator navigator;
    
    public ForgotPasswordViewModel(
        IAuthenticationService authenticationService,
        IErrorHandler errorHandler,
        INavigator navigator,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        this.authenticationService = authenticationService;
        this.navigator = navigator;
        InitializedCommand = SpravyCommand.Create(InitializedAsync, errorHandler, taskProgressService);
        ForgotPasswordCommand = SpravyCommand.Create(ForgotPasswordAsync, errorHandler, taskProgressService);
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
    
    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken ct)
    {
        switch (IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService.UpdateVerificationCodeByEmailAsync(Identifier, ct);
            case UserIdentifierType.Login:
                return authenticationService.UpdateVerificationCodeByLoginAsync(Identifier, ct);
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess
           .IfSuccessAsync(() =>
            {
                switch (IdentifierType)
                {
                    case UserIdentifierType.Email:
                        return authenticationService.UpdatePasswordByEmailAsync(Identifier,
                            VerificationCode.ToUpperInvariant(), NewPassword, ct);
                    case UserIdentifierType.Login:
                        return authenticationService.UpdatePasswordByLoginAsync(Identifier,
                            VerificationCode.ToUpperInvariant(), NewPassword, ct);
                    default: throw new ArgumentOutOfRangeException();
                }
            }, ct)
           .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct);
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