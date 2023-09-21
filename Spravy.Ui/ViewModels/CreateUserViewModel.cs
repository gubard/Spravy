using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
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
        CreateUserCommand = CreateCommandFromTaskWithDialogProgressIndicator(CreateUserAsync);
        EnterCommand = CreateCommandFromTaskWithDialogProgressIndicator<CreateUserView>(EnterAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public ICommand CreateUserCommand { get; }
    public ICommand EnterCommand { get; }

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

    private async Task EnterAsync(CreateUserView view)
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

        await CreateUserAsync();
    }

    private async Task CreateUserAsync()
    {
        if (Password != RepeatPassword)
        {
            throw new Exception("Password not equal RepeatPassword.");
        }

        var options = Mapper.Map<CreateUserOptions>(this);
        await AuthenticationService.CreateUserAsync(options);
        Navigator.NavigateTo<LoginViewModel>();
    }
}