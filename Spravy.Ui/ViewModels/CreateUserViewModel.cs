using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CreateUserViewModel : NavigatableViewModelBase
{
    private string login = string.Empty;
    private string password = string.Empty;
    private string repeatPassword = string.Empty;
    private string email = string.Empty;

    public CreateUserViewModel() : base(true)
    {
        CreateUserCommand = CreateCommandFromTask(TaskWork.Create(CreateUserAsync).RunAsync);
        EnterCommand = CreateCommandFromTask<CreateUserView>(TaskWork.Create<CreateUserView>(EnterAsync).RunAsync);
        BackCommand = CreateCommandFromTask(TaskWork.Create(BackAsync).RunAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public ICommand CreateUserCommand { get; }
    public ICommand EnterCommand { get; }
    public ICommand BackCommand { get; }

    public string Email
    {
        get => email;
        set => this.RaiseAndSetIfChanged(ref email, value);
    }

    public string Login
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public string RepeatPassword
    {
        get => repeatPassword;
        set => this.RaiseAndSetIfChanged(ref repeatPassword, value);
    }

    private async Task EnterAsync(CreateUserView view, CancellationToken cancellationToken)
    {
        var emailTextBox = view.FindControl<TextBox>(CreateUserView.EmailTextBoxName);

        if (emailTextBox is null)
        {
            return;
        }

        var loginTextBox = view.FindControl<TextBox>(CreateUserView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return;
        }

        if (emailTextBox.IsFocused)
        {
            loginTextBox.Focus();

            return;
        }

        var passwordTextBox = view.FindControl<TextBox>(CreateUserView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return;
        }

        var repeatPasswordTextBox = view.FindControl<TextBox>(CreateUserView.RepeatPasswordTextBoxName);

        if (repeatPasswordTextBox is null)
        {
            return;
        }

        if (passwordTextBox.IsFocused)
        {
            repeatPasswordTextBox.Focus();

            return;
        }

        await CreateUserAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task CreateUserAsync(CancellationToken cancellationToken)
    {
        if (Password != RepeatPassword)
        {
            throw new("Password not equal RepeatPassword.");
        }

        var options = Mapper.Map<CreateUserOptions>(this);
        cancellationToken.ThrowIfCancellationRequested();
        await AuthenticationService.CreateUserAsync(options, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        await Navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
    }

    private async Task BackAsync(CancellationToken cancellationToken)
    {
        await Navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
    }
}