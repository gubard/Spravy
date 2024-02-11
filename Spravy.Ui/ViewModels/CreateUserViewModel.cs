using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
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
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CreateUserViewModel : NavigatableViewModelBase, ICreateUserProperties, INotifyDataErrorInfo
{
    private string login = string.Empty;
    private string password = string.Empty;
    private string repeatPassword = string.Empty;
    private string email = string.Empty;
    private bool emailChanged;
    private bool loginChanged;
    private bool passwordChanged;
    private bool repeatPasswordChanged;
    private bool isBusy;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public CreateUserViewModel() : base(true)
    {
        var createUserWork = TaskWork.Create(CreateUserAsync);
        EnterCommand = CreateCommandFromTask<CreateUserView>(TaskWork.Create<CreateUserView>(EnterAsync).RunAsync);

        this.WhenAnyValue(x => x.Email)
            .Skip(1)
            .Subscribe(
                _ =>
                {
                    emailChanged = true;
                    this.RaisePropertyChanged(nameof(HasErrors));
                }
            );

        this.WhenAnyValue(x => x.Login)
            .Skip(1)
            .Subscribe(
                _ =>
                {
                    loginChanged = true;
                    this.RaisePropertyChanged(nameof(HasErrors));
                }
            );

        this.WhenAnyValue(x => x.Password)
            .Skip(1)
            .Subscribe(
                _ =>
                {
                    passwordChanged = true;
                    this.RaisePropertyChanged(nameof(HasErrors));
                }
            );

        this.WhenAnyValue(x => x.RepeatPassword)
            .Skip(1)
            .Subscribe(
                _ =>
                {
                    repeatPasswordChanged = true;
                    this.RaisePropertyChanged(nameof(HasErrors));
                }
            );

        CreateUserCommand = CreateCommandFromTask(
            createUserWork.RunAsync,
            this.WhenAnyValue(x => x.HasErrors).Select(x => !x)
        );
    }

    [Inject]
    public required IPropertyValidator PropertyValidator { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public ICommand EnterCommand { get; }
    public ICommand CreateUserCommand { get; }
    public override string ViewId => TypeCache<CreateUserViewModel>.Type.Name;

    public bool HasErrors
    {
        get
        {
            if (PropertyValidator is null)
            {
                return true;
            }

            var hasError = PropertyValidator.ValidLogin(Login, nameof(Login)) is not null
                           || PropertyValidator.ValidLength(Login, 4, 512, nameof(Login)) is not null
                           || PropertyValidator.ValidEmail(Email, nameof(Email)) is not null
                           || PropertyValidator.ValidLength(Email, 6, 50, nameof(Email)) is not null
                           || PropertyValidator.ValidPassword(Password, nameof(Password)) is not null
                           || PropertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null
                           || PropertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword)) is not null
                           || PropertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword)) is not null
                           || PropertyValidator.ValidEquals(
                               Password,
                               RepeatPassword,
                               nameof(Password),
                               nameof(RepeatPassword)
                           ) is not null;

            return hasError;
        }
    }

    public bool IsBusy
    {
        get => isBusy;
        set => this.RaiseAndSetIfChanged(ref isBusy, value);
    }

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

        if (HasErrors)
        {
            return;
        }

        await this.InvokeUIAsync(() => CreateUserCommand.Execute(null));
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

    public IEnumerable GetErrors(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Login):
            {
                if (loginChanged)
                {
                    var valid = PropertyValidator.ValidLogin(Login, nameof(Login));
                    var validLength = PropertyValidator.ValidLength(Login, 4, 512, nameof(Login));

                    if (valid is not null)
                    {
                        yield return valid;
                    }

                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                }

                break;
            }
            case nameof(Email):
            {
                if (emailChanged)
                {
                    var valid = PropertyValidator.ValidEmail(Email, nameof(Email));
                    var validLength = PropertyValidator.ValidLength(Email, 6, 50, nameof(Email));

                    if (valid is not null)
                    {
                        yield return valid;
                    }

                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                }

                break;
            }
            case nameof(Password):
            {
                if (passwordChanged)
                {
                    var valid = PropertyValidator.ValidPassword(Password, nameof(Password));
                    var validLength = PropertyValidator.ValidLength(Password, 8, 512, nameof(Password));

                    if (valid is not null)
                    {
                        yield return valid;
                    }

                    if (validLength is not null)
                    {
                        yield return validLength;
                    }
                }

                break;
            }
            case nameof(RepeatPassword):
            {
                if (repeatPasswordChanged)
                {
                    var valid = PropertyValidator.ValidPassword(RepeatPassword, nameof(RepeatPassword));
                    var validLength = PropertyValidator.ValidLength(RepeatPassword, 8, 512, nameof(RepeatPassword));

                    var validEquals = PropertyValidator.ValidEquals(
                        Password,
                        RepeatPassword,
                        nameof(Password),
                        nameof(RepeatPassword)
                    );

                    if (valid is not null)
                    {
                        yield return valid;
                    }

                    if (validLength is not null)
                    {
                        yield return validLength;
                    }

                    if (validEquals is not null)
                    {
                        yield return validEquals;
                    }
                }

                break;
            }
        }
    }

    private async Task CreateUserAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.InvokeUIAsync(() => IsBusy = true);
            var options = Mapper.Map<CreateUserOptions>(this);
            cancellationToken.ThrowIfCancellationRequested();
            await AuthenticationService.CreateUserAsync(options, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                    vm =>
                    {
                        vm.Identifier = Email;
                        vm.IdentifierType = UserIdentifierType.Email;
                    },
                    cancellationToken
                )
                .ConfigureAwait(false);
        }
        finally
        {
            await this.InvokeUIAsync(() => IsBusy = false);
        }
    }
}