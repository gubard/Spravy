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
using ReactiveUI.Fody.Helpers;
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
using Spravy.Ui.Services;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class LoginViewModel : NavigatableViewModelBase, ILoginProperties, INotifyDataErrorInfo
{
    private bool loginChanged;
    private bool passwordChanged;

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

    [Reactive]
    public bool IsBusy { get; set; }

    [Reactive]
    public bool TryAutoLogin { get; set; }

    [Reactive]
    public bool IsRememberMe { get; set; }

    [Reactive]
    public string Login { get; set; } = string.Empty;

    [Reactive]
    public string Password { get; set; } = string.Empty;

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

    private ValueTask<Result> LoginAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .ConfigureAwait(false)
            .IfSuccessTryFinallyAsync(
                () => AuthenticationService.IsVerifiedByLoginAsync(Login, cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        isVerified =>
                        {
                            if (!isVerified)
                            {
                                return Navigator.NavigateToAsync<VerificationCodeViewModel>(
                                        vm =>
                                        {
                                            vm.Identifier = Login;
                                            vm.IdentifierType = UserIdentifierType.Login;
                                        },
                                        cancellationToken
                                    )
                                    .ConfigureAwait(false);
                            }

                            var user = Mapper.Map<User>(this);

                            return TokenService.LoginAsync(user, cancellationToken)
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    () => this.InvokeUIBackgroundAsync(() => Account.Login = user.Login)
                                        .ConfigureAwait(false)
                                        .IfSuccessAsync(
                                            () => RememberMeAsync(cancellationToken).ConfigureAwait(false)
                                        )
                                        .ConfigureAwait(false)
                                        .IfSuccessAsync(
                                            () => Navigator.NavigateToAsync(
                                                    ActionHelper<RootToDoItemsViewModel>.Empty,
                                                    cancellationToken
                                                )
                                                .ConfigureAwait(false)
                                        )
                                        .ConfigureAwait(false)
                                )
                                .ConfigureAwait(false);
                        }
                    )
                    .ConfigureAwait(false),
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ConfigureAwait(false)
                    .ToValueTask()
                    .ConfigureAwait(false)
            );
    }

    private ValueTask<Result> RememberMeAsync(CancellationToken cancellationToken)
    {
        if (!IsRememberMe)
        {
            return Result.SuccessValueTask;
        }

        return TokenService.GetTokenAsync(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                token =>
                {
                    var item = new LoginStorageItem
                    {
                        Token = token,
                    };

                    return ObjectStorage.SaveObjectAsync(StorageIds.LoginId, item).ConfigureAwait(false);
                }
            );
    }

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .ConfigureAwait(false)
            .IfSuccessTryFinallyAsync(
                () =>
                {
                    if (!TryAutoLogin)
                    {
                        return Result.AwaitableFalse;
                    }

                    return ObjectStorage.GetObjectOrDefaultAsync<LoginViewModelSetting>(ViewId)
                        .ConfigureAwait(false)
                        .IfSuccessAsync(
                            setting =>
                            {
                                return SetStateAsync(setting)
                                    .ConfigureAwait(false)
                                    .IfSuccessAsync(
                                        () => ObjectStorage.IsExistsAsync(StorageIds.LoginId).ConfigureAwait(false)
                                    )
                                    .ConfigureAwait(false)
                                    .IfSuccessAsync(
                                        value =>
                                        {
                                            if (!value)
                                            {
                                                return Result.AwaitableFalse;
                                            }

                                            return ObjectStorage.GetObjectAsync<LoginStorageItem>(StorageIds.LoginId)
                                                .ConfigureAwait(false)
                                                .IfSuccessAsync(
                                                    item =>
                                                    {
                                                        var jwtHandler = new JwtSecurityTokenHandler();
                                                        var jwtToken = jwtHandler.ReadJwtToken(item.Token);
                                                        var l = jwtToken.Claims.Single(x => x.Type == ClaimTypes.Name)
                                                            .Value;
                                                        Account.Login = l;

                                                        return TokenService.LoginAsync(
                                                                item.Token.ThrowIfNullOrWhiteSpace(),
                                                                cancellationToken
                                                            )
                                                            .ConfigureAwait(false)
                                                            .IfSuccessAsync(
                                                                () => Navigator.NavigateToAsync(
                                                                        ActionHelper<RootToDoItemsViewModel>.Empty,
                                                                        cancellationToken
                                                                    )
                                                                    .ConfigureAwait(false)
                                                            )
                                                            .ConfigureAwait(false);
                                                    }
                                                )
                                                .ConfigureAwait(false);
                                        }
                                    )
                                    .ConfigureAwait(false);
                            }
                        )
                        .ConfigureAwait(false);
                },
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ConfigureAwait(false)
                    .ToValueTask()
                    .ConfigureAwait(false)
            );
    }

    private ValueTask<Result> EnterAsync(LoginView view, CancellationToken cancellationToken)
    {
        var loginTextBox = view.FindControl<TextBox>(LoginView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return Result.SuccessValueTask;
        }

        var passwordTextBox = view.FindControl<TextBox>(LoginView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return Result.SuccessValueTask;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return Result.SuccessValueTask;
        }

        if (HasErrors)
        {
            return Result.SuccessValueTask;
        }

        return this.InvokeUIBackgroundAsync(() => LoginCommand.Execute(null));
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this));
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return setting.CastObject<LoginViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            TryAutoLogin = false;
                            Login = s.Login;
                        }
                    )
                    .ConfigureAwait(false)
            );
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