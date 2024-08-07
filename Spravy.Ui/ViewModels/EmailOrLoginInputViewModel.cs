using Spravy.Ui.Setting;

namespace Spravy.Ui.ViewModels;

public partial class EmailOrLoginInputViewModel : NavigatableViewModelBase
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string emailOrLogin = string.Empty;

    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly IAuthenticationService authenticationService;

    public EmailOrLoginInputViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        IObjectStorage objectStorage,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService
    )
        : base(true)
    {
        this.navigator = navigator;
        this.objectStorage = objectStorage;
        this.authenticationService = authenticationService;

        ForgotPasswordCommand = SpravyCommand.Create(
            ForgotPasswordAsync,
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand ForgotPasswordCommand { get; }

    public override string ViewId
    {
        get => TypeCache<EmailOrLoginInputViewModel>.Type.Name;
    }

    private ConfiguredValueTaskAwaitable<Result> ForgotPasswordAsync(CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                IsBusy = true;

                return Result.Success;
            })
            .IfSuccessTryFinallyAsync(
                () =>
                {
                    if (EmailOrLogin.Contains('@'))
                    {
                        return authenticationService
                            .IsVerifiedByEmailAsync(EmailOrLogin, ct)
                            .IfSuccessAsync(
                                value =>
                                {
                                    if (value)
                                    {
                                        return navigator.NavigateToAsync<ForgotPasswordViewModel>(
                                            vm =>
                                            {
                                                vm.Identifier = EmailOrLogin;
                                                vm.IdentifierType = UserIdentifierType.Email;
                                            },
                                            ct
                                        );
                                    }

                                    return navigator.NavigateToAsync<VerificationCodeViewModel>(
                                        vm =>
                                        {
                                            vm.IdentifierType = UserIdentifierType.Email;
                                            vm.Identifier = EmailOrLogin;
                                        },
                                        ct
                                    );
                                },
                                ct
                            );
                    }

                    return authenticationService
                        .IsVerifiedByLoginAsync(EmailOrLogin, ct)
                        .IfSuccessAsync(
                            value =>
                            {
                                if (value)
                                {
                                    return navigator.NavigateToAsync<ForgotPasswordViewModel>(
                                        vm =>
                                        {
                                            vm.Identifier = EmailOrLogin;
                                            vm.IdentifierType = UserIdentifierType.Login;
                                        },
                                        ct
                                    );
                                }

                                return navigator.NavigateToAsync<VerificationCodeViewModel>(
                                    vm =>
                                    {
                                        vm.IdentifierType = UserIdentifierType.Login;
                                        vm.Identifier = EmailOrLogin;
                                    },
                                    ct
                                );
                            },
                            ct
                        );
                },
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                        {
                            IsBusy = false;

                            return Result.Success;
                        })
                        .ToValueTask()
                        .ConfigureAwait(false),
                ct
            );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(
            ViewId,
            new EmailOrLoginInputViewModelSetting(this),
            ct
        );
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken ct
    )
    {
        var s = setting.ThrowIfIsNotCast<EmailOrLoginInputViewModelSetting>();

        return this.InvokeUiBackgroundAsync(() =>
        {
            EmailOrLogin = s.Identifier;

            return Result.Success;
        });
    }
}
