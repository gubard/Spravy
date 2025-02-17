using Spravy.Ui.Setting;

namespace Spravy.Ui.ViewModels;

public partial class EmailOrLoginInputViewModel : NavigatableViewModelBase
{
    private readonly IAuthenticationService authenticationService;

    private readonly INavigator navigator;
    private readonly IObjectStorage objectStorage;
    private readonly IViewFactory viewFactory;

    [ObservableProperty]
    private string emailOrLogin = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public EmailOrLoginInputViewModel(
        IErrorHandler errorHandler,
        INavigator navigator,
        IObjectStorage objectStorage,
        IAuthenticationService authenticationService,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory
    ) : base(true)
    {
        this.navigator = navigator;
        this.objectStorage = objectStorage;
        this.authenticationService = authenticationService;
        this.viewFactory = viewFactory;
        ForgotPasswordCommand = SpravyCommand.Create(ForgotPasswordAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand ForgotPasswordCommand { get; }
    public override string ViewId => TypeCache<EmailOrLoginInputViewModel>.Name;

    private Cvtar ForgotPasswordAsync(CancellationToken ct)
    {
        return this.PostUiBackground(
                () =>
                {
                    IsBusy = true;

                    return Result.Success;
                },
                ct
            )
           .IfSuccessTryFinallyAsync(
                () =>
                {
                    if (EmailOrLogin.Contains('@'))
                    {
                        return authenticationService.IsVerifiedByEmailAsync(EmailOrLogin, ct)
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

                    return authenticationService.IsVerifiedByLoginAsync(EmailOrLogin, ct)
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
                                    viewFactory.CreateVerificationCodeViewModel(EmailOrLogin, UserIdentifierType.Email),
                                    ct
                                );
                            },
                            ct
                        );
                },
                () => this.PostUiBackground(
                        () =>
                        {
                            IsBusy = false;

                            return Result.Success;
                        },
                        ct
                    )
                   .GetAwaitable(),
                ct
            );
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new EmailOrLoginInputViewModelSetting(this), ct);
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