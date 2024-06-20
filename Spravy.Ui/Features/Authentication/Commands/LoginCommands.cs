using Spravy.Ui.Mappers;

namespace Spravy.Ui.Features.Authentication.Commands;

public class LoginCommands
{
    private readonly IAuthenticationService authenticationService;
    private readonly INavigator navigator;
    private readonly AccountNotify account;
    private readonly ITokenService tokenService;
    private readonly IObjectStorage objectStorage;

    public LoginCommands(
        IAuthenticationService authenticationService,
        INavigator navigator,
        AccountNotify account,
        ITokenService tokenService,
        IObjectStorage objectStorage,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.authenticationService = authenticationService;
        this.navigator = navigator;
        this.account = account;
        this.tokenService = tokenService;
        this.objectStorage = objectStorage;
        Initialized = SpravyCommand.Create<LoginViewModel>(InitializedAsync, errorHandler, taskProgressService);
        Enter = SpravyCommand.Create<LoginView>(EnterAsync, errorHandler, taskProgressService);
        Login = SpravyCommand.Create<LoginViewModel>(LoginAsync, errorHandler, taskProgressService);

        ForgotPassword =
            SpravyCommand.CreateNavigateTo<ForgotPasswordViewModel>(navigator, errorHandler, taskProgressService);

        CreateUser = SpravyCommand.CreateNavigateTo<CreateUserViewModel>(navigator, errorHandler, taskProgressService);
    }

    public SpravyCommand Initialized { get; }
    public SpravyCommand Enter { get; }
    public SpravyCommand Login { get; }
    public SpravyCommand ForgotPassword { get; }
    public SpravyCommand CreateUser { get; }

    private ConfiguredValueTaskAwaitable<Result> LoginAsync(
        LoginViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() => authenticationService
               .IsVerifiedByLoginAsync(viewModel.Login, cancellationToken)
               .IfSuccessAsync(isVerified =>
                {
                    if (!isVerified)
                    {
                        return navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                        {
                            vm.Identifier = viewModel.Login;
                            vm.IdentifierType = UserIdentifierType.Login;
                        }, cancellationToken);
                    }

                    return tokenService.LoginAsync(viewModel.ToUser(), cancellationToken)
                       .IfSuccessAsync(() => this.InvokeUiBackgroundAsync(() =>
                            {
                                account.Login = viewModel.Login;

                                return Result.Success;
                            })
                           .IfSuccessAsync(() => RememberMeAsync(viewModel, cancellationToken), cancellationToken)
                           .IfSuccessAsync(
                                () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty,
                                    cancellationToken),
                                cancellationToken), cancellationToken);
                }, cancellationToken), () => this.InvokeUiBackgroundAsync(() =>
                {
                    viewModel.IsBusy = false;

                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RememberMeAsync(
        LoginViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        if (!viewModel.IsRememberMe)
        {
            return Result.AwaitableSuccess;
        }

        return tokenService.GetTokenAsync(cancellationToken)
           .IfSuccessAsync(token =>
            {
                var item = new LoginStorageItem
                {
                    Token = token,
                };

                return objectStorage.SaveObjectAsync(StorageIds.LoginId, item);
            }, cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(
        LoginViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() =>
            {
                return objectStorage.GetObjectOrDefaultAsync<LoginViewModelSetting>(viewModel.ViewId, cancellationToken)
                   .IfSuccessAsync(setting =>
                    {
                        return viewModel.SetStateAsync(setting, cancellationToken)
                           .IfSuccessAsync(() => objectStorage.IsExistsAsync(StorageIds.LoginId), cancellationToken)
                           .IfSuccessAsync(value =>
                            {
                                if (!value)
                                {
                                    return Result.AwaitableSuccess;
                                }

                                return objectStorage.GetObjectAsync<LoginStorageItem>(StorageIds.LoginId)
                                   .IfSuccessAsync(item =>
                                    {
                                        var jwtHandler = new JwtSecurityTokenHandler();
                                        var jwtToken = jwtHandler.ReadJwtToken(item.Token);
                                        var l = jwtToken.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
                                        account.Login = l;

                                        return tokenService.LoginAsync(item.Token.ThrowIfNullOrWhiteSpace(),
                                                cancellationToken)
                                           .IfSuccessAsync(
                                                () => navigator.NavigateToAsync(
                                                    ActionHelper<RootToDoItemsViewModel>.Empty,
                                                    cancellationToken), cancellationToken);
                                    }, cancellationToken);
                            }, cancellationToken);
                    }, cancellationToken);
            }, () => this.InvokeUiBackgroundAsync(() =>
                {
                    viewModel.IsBusy = false;

                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> EnterAsync(LoginView view, CancellationToken cancellationToken)
    {
        return this.InvokeUiAsync(() =>
                view.FindControl<TextBox>(LoginView.LoginTextBoxName).IfNotNull(nameof(LoginView.LoginTextBoxName)))
           .IfSuccessAsync(loginTextBox => this
               .InvokeUiAsync(() =>
                    view.FindControl<TextBox>(LoginView.PasswordTextBoxName).IfNotNull(LoginView.PasswordTextBoxName))
               .IfSuccessAsync(passwordTextBox =>
                {
                    if (loginTextBox.IsFocused)
                    {
                        return this.InvokeUiAsync(() =>
                        {
                            passwordTextBox.Focus();

                            return Result.Success;
                        });
                    }

                    if (passwordTextBox.IsFocused)
                    {
                        return this.InvokeUiAsync(() => view.ViewModel.IfNotNull(nameof(view.ViewModel)))
                           .IfSuccessAsync(viewModel =>
                            {
                                if (viewModel.HasErrors)
                                {
                                    return Result.AwaitableSuccess;
                                }

                                return LoginAsync(viewModel, cancellationToken);
                            }, cancellationToken);
                    }

                    return Result.AwaitableSuccess;
                }, cancellationToken), cancellationToken);
    }
}