using Spravy.Ui.Mappers;

namespace Spravy.Ui.Features.Authentication.Commands;

public class CreateUserCommands
{
    private readonly IAuthenticationService authenticationService;
    private readonly INavigator navigator;

    public CreateUserCommands(
        IAuthenticationService authenticationService,
        INavigator navigator,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        this.authenticationService = authenticationService;
        this.navigator = navigator;
        EnterCommand = SpravyCommand.Create<CreateUserView>(EnterAsync, errorHandler, taskProgressService);

        CreateUserCommand =
            SpravyCommand.Create<CreateUserViewModel>(CreateUserAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand EnterCommand { get; }
    public SpravyCommand CreateUserCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> EnterAsync(CreateUserView view, CancellationToken ct)
    {
        return this.InvokeUiAsync(() => view.FindControl<TextBox>(CreateUserView.EmailTextBoxName)
               .IfNotNull(CreateUserView.EmailTextBoxName)
               .IfSuccess(emailTextBox => view.FindControl<TextBox>(CreateUserView.LoginTextBoxName)
                   .IfNotNull(CreateUserView.LoginTextBoxName)
                   .IfSuccess(loginTextBox => view.FindControl<TextBox>(CreateUserView.PasswordTextBoxName)
                       .IfNotNull(CreateUserView.PasswordTextBoxName)
                       .IfSuccess(passwordTextBox => view.FindControl<TextBox>(CreateUserView.RepeatPasswordTextBoxName)
                           .IfNotNull(CreateUserView.RepeatPasswordTextBoxName)
                           .IfSuccess(repeatPasswordTextBox => new
                            {
                                EmailTextBox = emailTextBox,
                                PasswordTextBox = passwordTextBox,
                                LoginTextBox = loginTextBox,
                                RepeatPasswordTextBox = repeatPasswordTextBox,
                            }.ToResult())))))
           .IfSuccessAsync(controls =>
            {
                if (controls.EmailTextBox.IsFocused)
                {
                    return this.InvokeUiAsync(() =>
                    {
                        controls.LoginTextBox.Focus();

                        return Result.Success;
                    });
                }

                if (controls.LoginTextBox.IsFocused)
                {
                    return this.InvokeUiAsync(() =>
                    {
                        controls.PasswordTextBox.Focus();

                        return Result.Success;
                    });
                }

                if (controls.PasswordTextBox.IsFocused)
                {
                    return this.InvokeUiAsync(() =>
                    {
                        controls.RepeatPasswordTextBox.Focus();

                        return Result.Success;
                    });
                }

                return this.InvokeUiAsync(() => view.ViewModel.IfNotNull(nameof(view.ViewModel)))
                   .IfSuccessAsync(vm =>
                    {
                        if (vm.HasErrors)
                        {
                            return Result.AwaitableSuccess;
                        }

                        return CreateUserAsync(vm, ct);
                    }, ct);
            }, ct);
    }

    private ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserViewModel viewModel,
        CancellationToken ct
    )
    {
        if (viewModel.HasErrors)
        {
            return Result.AwaitableSuccess;
        }

        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() => authenticationService
               .CreateUserAsync(viewModel.ToCreateUserOptions(), ct)
               .IfSuccessAsync(() => navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                {
                    vm.Identifier = viewModel.Email;
                    vm.IdentifierType = UserIdentifierType.Email;
                }, ct), ct), () => this.InvokeUiBackgroundAsync(() =>
                {
                    viewModel.IsBusy = false;

                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), ct);
    }
}