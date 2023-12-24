using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class LoginViewModel : NavigatableViewModelBase, ILoginProperties
{
    private string login = string.Empty;
    private string password = string.Empty;
    private bool isRememberMe;

    public LoginViewModel() : base(false)
    {
        EnterCommand = CreateCommandFromTask<LoginView>(TaskWork.Create<LoginView>(EnterAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    [Inject]
    public required AccountNotify Account { get; init; }

    [Inject]
    public required ITokenService TokenService { get; init; }

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
    public ICommand EnterCommand { get; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        if (!await ObjectStorage.IsExistsAsync(FileIds.LoginFileId).ConfigureAwait(false))
        {
            return;
        }

        var item = await ObjectStorage.GetObjectAsync<LoginStorageItem>(FileIds.LoginFileId).ConfigureAwait(false);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(item.Token);
        var l = jwtToken.Claims.Single(x => x.Type == ClaimTypes.Name).Value;
        Account.Login = l;
        await TokenService.LoginAsync(item.Token.ThrowIfNullOrWhiteSpace(), cancellationToken).ConfigureAwait(false);

        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
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

        await this.InvokeUIAsync(() => CommandStorage.LoginCommand.Execute(this));
    }

    public override void Stop()
    {
    }
}