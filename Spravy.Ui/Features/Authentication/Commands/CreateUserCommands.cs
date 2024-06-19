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
        CreateUserCommand = SpravyCommand.Create<CreateUserViewModel>(CreateUserAsync, errorHandler, taskProgressService);
    }

    public SpravyCommand EnterCommand { get; }
    public SpravyCommand CreateUserCommand { get; }

    private ConfiguredValueTaskAwaitable<Result> EnterAsync(CreateUserView view, CancellationToken cancellationToken)
    {
        var emailTextBox = view.FindControl<TextBox>(CreateUserView.EmailTextBoxName);

        if (emailTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        var loginTextBox = view.FindControl<TextBox>(CreateUserView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (emailTextBox.IsFocused)
        {
            loginTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        var passwordTextBox = view.FindControl<TextBox>(CreateUserView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        var repeatPasswordTextBox = view.FindControl<TextBox>(CreateUserView.RepeatPasswordTextBoxName);

        if (repeatPasswordTextBox is null)
        {
            return Result.AwaitableSuccess;
        }

        if (passwordTextBox.IsFocused)
        {
            repeatPasswordTextBox.Focus();

            return Result.AwaitableSuccess;
        }

        return view.ViewModel
           .IfNotNull(nameof(view.ViewModel))
           .IfSuccessAsync(vm =>
            {
                if (vm.HasErrors)
                {
                    return Result.AwaitableSuccess;
                }

                return CreateUserAsync(vm, cancellationToken);
            }, cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
        CreateUserViewModel viewModel,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
           .IfSuccessTryFinallyAsync(() => authenticationService
               .CreateUserAsync(viewModel.ToCreateUserOptions(), cancellationToken)
               .IfSuccessAsync(() => navigator.NavigateToAsync<VerificationCodeViewModel>(vm =>
                {
                    vm.Identifier = viewModel.Email;
                    vm.IdentifierType = UserIdentifierType.Email;
                }, cancellationToken), cancellationToken), () => this.InvokeUiBackgroundAsync(() =>
                {
                    viewModel.IsBusy = false;

                    return Result.Success;
                })
               .ToValueTask()
               .ConfigureAwait(false), cancellationToken);
    }
}