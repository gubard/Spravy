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

public class CreateUserViewModel : RoutableViewModelBase
{
    private string login = string.Empty;
    private string password = string.Empty;
    private string repeatPassword = string.Empty;

    public CreateUserViewModel() : base("create-user")
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
        var loginTextBox = view.FindControl<TextBox>(CreateUserView.LoginTextBoxName);

        if (loginTextBox is null)
        {
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

        await CreateUserAsync(cancellationToken);
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
        await Navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken);
    }

    private Task BackAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken);
    }
}