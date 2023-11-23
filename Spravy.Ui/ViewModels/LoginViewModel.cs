using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Helpers;
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
        LoginCommand = CreateCommandFromTask(TaskWork.Create(LoginAsync).RunAsync);
        CreateUserCommand = CreateCommandFromTask(TaskWork.Create(CreateUserAsync).RunAsync);
        EnterCommand = CreateCommandFromTask<LoginView>(TaskWork.Create<LoginView>(EnterAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required ITokenService TokenService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

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

    public ICommand InitializedCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand EnterCommand { get; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        if (!await ObjectStorage.IsExistsAsync(FileIds.LoginFileId).ConfigureAwait(false))
        {
            return;
        }

        var item = await ObjectStorage.GetObjectAsync<LoginStorageItem>(FileIds.LoginFileId).ConfigureAwait(false);
        await TokenService.LoginAsync(item.Token.ThrowIfNullOrWhiteSpace(), cancellationToken).ConfigureAwait(false);
        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
    }

    private async Task EnterAsync(LoginView view, CancellationToken cancellationToken)
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

        await LoginAsync(cancellationToken);
    }

    private Task CreateUserAsync(CancellationToken cancellationToken)
    {
        return Navigator.NavigateToAsync(ActionHelper<CreateUserViewModel>.Empty, cancellationToken);
    }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var user = Mapper.Map<User>(this);
        await TokenService.LoginAsync(user, cancellationToken).ConfigureAwait(false);
        await RememberMeAsync(cancellationToken);
        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
    }

    private async Task RememberMeAsync(CancellationToken cancellationToken)
    {
        if (!IsRememberMe)
        {
            return;
        }

        var token = await TokenService.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        var item = new LoginStorageItem
        {
            Token = token,
        };

        await ObjectStorage.SaveObjectAsync(FileIds.LoginFileId, item).ConfigureAwait(false);
    }
}