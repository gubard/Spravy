namespace Spravy.Ui.ViewModels;

public class EmailOrLoginInputViewModel : NavigatableViewModelBase
{
    public EmailOrLoginInputViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<EmailOrLoginInputViewModel>.Type.Name;
    }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Reactive]
    public ICommand ForgotPasswordCommand { get; protected set; }

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public string EmailOrLogin { get; set; } = string.Empty;

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        ForgotPasswordCommand = CreateCommandFromTask(TaskWork.Create(ForgotPasswordAsync).RunAsync);

        return Result.AwaitableFalse;
    }

    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
           .IfSuccessTryFinallyAsync(() =>
                {
                    if (EmailOrLogin.Contains('@'))
                    {
                        return AuthenticationService.IsVerifiedByEmailAsync(EmailOrLogin, cancellationToken)
                           .IfSuccessAsync(value =>
                            {
                                if (value)
                                {
                                    return AuthenticationService
                                       .UpdateVerificationCodeByEmailAsync(EmailOrLogin, cancellationToken)
                                       .IfSuccessAsync(() => Navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
                                        {
                                            vm.Identifier = EmailOrLogin;

                                            vm.IdentifierType = EmailOrLogin.Contains('@')
                                                ? UserIdentifierType.Email
                                                : UserIdentifierType.Login;
                                        }, cancellationToken), cancellationToken);
                                }

                                return Navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                                {
                                    vm.IdentifierType = UserIdentifierType.Email;
                                    vm.Identifier = EmailOrLogin;
                                }, cancellationToken);
                            }, cancellationToken);
                    }

                    return AuthenticationService.IsVerifiedByLoginAsync(EmailOrLogin, cancellationToken)
                       .IfSuccessAsync(value =>
                        {
                            if (value)
                            {
                                return AuthenticationService
                                   .UpdateVerificationCodeByLoginAsync(EmailOrLogin, cancellationToken)
                                   .IfSuccessAsync(() => Navigator.NavigateToAsync<ForgotPasswordViewModel>(vm =>
                                    {
                                        vm.Identifier = EmailOrLogin;

                                        vm.IdentifierType = EmailOrLogin.Contains('@')
                                            ? UserIdentifierType.Email
                                            : UserIdentifierType.Login;
                                    }, cancellationToken), cancellationToken);
                            }

                            return Navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                            {
                                vm.IdentifierType = UserIdentifierType.Login;
                                vm.Identifier = EmailOrLogin;
                            }, cancellationToken);
                        }, cancellationToken);
                }, () => this.InvokeUIBackgroundAsync(() => IsBusy = false).ToValueTask().ConfigureAwait(false),
                cancellationToken);

        ;
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new EmailOrLoginInputViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        var s = setting.ThrowIfIsNotCast<EmailOrLoginInputViewModelSetting>();

        return this.InvokeUIBackgroundAsync(() => EmailOrLogin = s.Identifier);
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