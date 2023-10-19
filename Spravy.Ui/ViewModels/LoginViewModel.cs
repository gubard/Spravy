using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private string login = string.Empty;
    private string password = string.Empty;
    private bool isRememberMe;

    public LoginViewModel() : base("login")
    {
        LoginCommand = CreateCommandFromTaskWithDialogProgressIndicator(LoginAsync);
        CreateUserCommand = CreateCommand(CreateUser);
        EnterCommand = CreateCommandFromTaskWithDialogProgressIndicator<LoginView>(EnterAsync);
    }

    [Inject]
    public required ITokenService TokenService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public bool IsRememberMe
    {
        get => isRememberMe;
        set => this.RaiseAndSetIfChanged(ref isRememberMe, value);
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

    public ICommand LoginCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand EnterCommand { get; }

    private async Task EnterAsync(LoginView view)
    {
        var loginTextBox = view.FindControl<TextBox>(LoginView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return;
        }

        var passwordTextBox = view.FindControl<TextBox>(LoginView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return;
        }

        await LoginAsync();
    }

    private void CreateUser()
    {
        Navigator.NavigateTo<CreateUserViewModel>();
    }

    private async Task LoginAsync()
    {
        Log.Logger.Information("Test {Number}", 1);
        var user = Mapper.Map<User>(this);
        Log.Logger.Information("Test {Number}", 2);
        await TokenService.LoginAsync(user);
        Log.Logger.Information("Test {Number}", 3);
        Navigator.NavigateTo<RootToDoItemViewModel>();
        Log.Logger.Information("Test {Number}", 4);
    }
}