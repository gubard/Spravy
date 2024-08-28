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
    private readonly IViewFactory viewFactory;

    public EmailOrLoginInputViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        IObjectStorage objectStorage,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory
    )
        : base(true)
    {
        this.navigator = navigator;
        this.objectStorage = objectStorage;
        this.authenticationService = authenticationService;
        this.viewFactory = viewFactory;

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

    private Cvtar ForgotPasswordAsync(CancellationToken ct)
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
                                        return navigator.NavigateToAsync(
                                            viewFactory.CreateForgotPasswordViewModel(
                                                EmailOrLogin,
                                                UserIdentifierType.Email
                                            ),
                                            ct
                                        );
                                    }

                                    return navigator.NavigateToAsync(
                                        viewFactory.CreateVerificationCodeViewModel(
                                            EmailOrLogin,
                                            UserIdentifierType.Email
                                        ),
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
                                    return navigator.NavigateToAsync(
                                        viewFactory.CreateForgotPasswordViewModel(
                                            EmailOrLogin,
                                            UserIdentifierType.Email
                                        ),
                                        ct
                                    );
                                }

                                return navigator.NavigateToAsync(
                                    viewFactory.CreateVerificationCodeViewModel(
                                        EmailOrLogin,
                                        UserIdentifierType.Email
                                    ),
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

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(
            ViewId,
            new EmailOrLoginInputViewModelSetting(this),
            ct
        );
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}
