using System;
using System.Collections;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ProtoBuf;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class LoginViewModel : NavigatableViewModelBase, ILoginProperties, INotifyDataErrorInfo
{
    private string login = string.Empty;
    private string password = string.Empty;
    private bool isRememberMe;
    private bool loginChanged;
    private bool passwordChanged;
    private bool tryAutoLogin;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public LoginViewModel() : base(true)
    {
        EnterCommand = CreateCommandFromTask<LoginView>(TaskWork.Create<LoginView>(EnterAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);

        LoginCommand = CreateCommandFromTask(
            TaskWork.Create(LoginAsync).RunAsync,
            this.WhenAnyValue(x => x.HasErrors).Select(x => !x)
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
    }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required AccountNotify Account { get; init; }

    [Inject]
    public required ITokenService TokenService { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IPropertyValidator PropertyValidator { get; init; }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    public bool TryAutoLogin
    {
        get => tryAutoLogin;
        set => this.RaiseAndSetIfChanged(ref tryAutoLogin, value);
    }

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
    public ICommand LoginCommand { get; }
    public override string ViewId => TypeCache<LoginViewModel>.Type.Name;

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
        }
    }

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
                           || PropertyValidator.ValidPassword(Password, nameof(Password)) is not null
                           || PropertyValidator.ValidLength(Password, 8, 512, nameof(Password)) is not null;

            return hasError;
        }
    }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var isVerified = await AuthenticationService.IsVerifiedByLoginAsync(Login, cancellationToken);

        if (!isVerified)
        {
            await Navigator.NavigateToAsync<VerificationCodeViewModel>(
                vm =>
                {
                    vm.Identifier = Login;
                    vm.IdentifierType = UserIdentifierType.Login;
                },
                cancellationToken
            );

            return;
        }

        var user = Mapper.Map<User>(this);
        await TokenService.LoginAsync(user, cancellationToken).ConfigureAwait(false);
        Account.Login = user.Login;
        await RememberMeAsync(cancellationToken);

        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
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

        await ObjectStorage.SaveObjectAsync(StorageIds.LoginId, item).ConfigureAwait(false);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        if (!TryAutoLogin)
        {
            return;
        }
        
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<LoginViewModelSetting>(ViewId).ConfigureAwait(false);
        await SetStateAsync(setting).ConfigureAwait(false);

        if (!await ObjectStorage.IsExistsAsync(StorageIds.LoginId).ConfigureAwait(false))
        {
            return;
        }

        var item = await ObjectStorage.GetObjectAsync<LoginStorageItem>(StorageIds.LoginId).ConfigureAwait(false);
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

        if (HasErrors)
        {
            return;
        }

        await this.InvokeUIAsync(() => LoginCommand.Execute(null));
    }

    public override void Stop()
    {
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<LoginViewModelSetting>();
        await this.InvokeUIBackgroundAsync(() => Login = s.Login);
    }

    [ProtoContract]
    class LoginViewModelSetting
    {
        public LoginViewModelSetting(LoginViewModel viewModel)
        {
            Login = viewModel.Login;
        }

        public LoginViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public string Login { get; set; } = string.Empty;
    }
}