namespace Spravy.Ui.ViewModels;

public class EmailOrLoginInputViewModel : NavigatableViewModelBase
{
    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly IAuthenticationService authenticationService;
    
    public EmailOrLoginInputViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        IObjectStorage objectStorage,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService
    ) : base(true)
    {
        this.navigator = navigator;
        this.objectStorage = objectStorage;
        this.authenticationService = authenticationService;
        ForgotPasswordCommand = SpravyCommand.Create(ForgotPasswordAsync, errorHandler, taskProgressService);
    }
    
    public SpravyCommand ForgotPasswordCommand { get; }
    
    public override string ViewId
    {
        get => TypeCache<EmailOrLoginInputViewModel>.Type.Name;
    }
    
    [Reactive]
    public bool IsBusy { get; set; }
    
    [Reactive]
    public string EmailOrLogin { get; set; } = string.Empty;
    
    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                IsBusy = true;
                
                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() =>
            {
                if (EmailOrLogin.Contains('@'))
                {
                    return authenticationService.IsVerifiedByEmailAsync(EmailOrLogin, cancellationToken)
                       .IfSuccessAsync(value =>
                        {
                            if (value)
                            {
                                return authenticationService
                                   .UpdateVerificationCodeByEmailAsync(EmailOrLogin, cancellationToken)
                                   .IfSuccessAsync(() => navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
                                    {
                                        vm.Identifier = EmailOrLogin;
                                        
                                        vm.IdentifierType = EmailOrLogin.Contains('@')
                                            ? UserIdentifierType.Email
                                            : UserIdentifierType.Login;
                                    }, cancellationToken), cancellationToken);
                            }
                            
                            return navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                            {
                                vm.IdentifierType = UserIdentifierType.Email;
                                vm.Identifier = EmailOrLogin;
                            }, cancellationToken);
                        }, cancellationToken);
                }
                
                return authenticationService.IsVerifiedByLoginAsync(EmailOrLogin, cancellationToken)
                   .IfSuccessAsync(value =>
                    {
                        if (value)
                        {
                            return authenticationService
                               .UpdateVerificationCodeByLoginAsync(EmailOrLogin, cancellationToken)
                               .IfSuccessAsync(() => navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
                                {
                                    vm.Identifier = EmailOrLogin;
                                    
                                    vm.IdentifierType = EmailOrLogin.Contains('@')
                                        ? UserIdentifierType.Email
                                        : UserIdentifierType.Login;
                                }, cancellationToken), cancellationToken);
                        }
                        
                        return navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                        {
                            vm.IdentifierType = UserIdentifierType.Login;
                            vm.Identifier = EmailOrLogin;
                        }, cancellationToken);
                    }, cancellationToken);
            }, () => this.InvokeUiBackgroundAsync(() =>
                {
                    IsBusy = false;
                    
                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), cancellationToken);
    }
    
    public override Result Stop()
    {
        return Result.Success;
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return objectStorage.SaveObjectAsync(ViewId, new EmailOrLoginInputViewModelSetting(this));
    }
    
    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        var s = setting.ThrowIfIsNotCast<EmailOrLoginInputViewModelSetting>();
        
        return this.InvokeUiBackgroundAsync(() =>
        {
            EmailOrLogin = s.Identifier;
            
            return Result.Success;
        });
    }
    
    [ProtoContract]
    private class EmailOrLoginInputViewModelSetting
    {
        public EmailOrLoginInputViewModelSetting()
        {
        }
        
        public EmailOrLoginInputViewModelSetting(EmailOrLoginInputViewModel viewModel)
        {
            Identifier = viewModel.EmailOrLogin;
        }
        
        [ProtoMember(1)]
        public string Identifier { get; set; } = string.Empty;
    }
}