using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CreateUserViewModel : NavigatableViewModelBase, ICreateUserProperties
{
    private string login = string.Empty;
    private string password = string.Empty;
    private string repeatPassword = string.Empty;
    private string email = string.Empty;

    public CreateUserViewModel() : base(true)
    {
        EnterCommand = CreateCommandFromTask<CreateUserView>(TaskWork.Create<CreateUserView>(EnterAsync).RunAsync);
    }

    public ICommand EnterCommand { get; }
    public override string ViewId => TypeCache<CreateUserViewModel>.Type.Name;

    [EmailAddress]
    [StringLength(50, ErrorMessage = "Email max length 50")]
    public string Email
    {
        get => email;
        set => this.RaiseAndSetIfChanged(ref email, value);
    }

    [StringLength(512, MinimumLength = 4)]
    [RegularExpression("^[!@#$%^&*()_+\\-={}\\[\\]:;\"'<>,.?/\\\\|a-zA-Z0-9]+$")]
    public string Login
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    [StringLength(512, MinimumLength = 8)]
    [RegularExpression("^[ !@#$%^&*()_+\\-={}\\[\\]:;\"'<>,.?/\\\\|a-zA-Z0-9]+$")]
    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    [StringLength(512, MinimumLength = 8)]
    [RegularExpression("^[ !@#$%^&*()_+\\-={}\\[\\]:;\"'<>,.?/\\\\|a-zA-Z0-9]+$")]
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

        await this.InvokeUIAsync(() => CommandStorage.CreateUserCommand.Execute(this));
    }

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    public override Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }
}