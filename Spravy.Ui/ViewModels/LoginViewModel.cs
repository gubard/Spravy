using System;
using System.Collections;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
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
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
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
    public ICommand EnterCommand { get; protected set; }
    public ICommand LoginCommand { get; protected set; }
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

    private ConfiguredValueTaskAwaitable<Result> LoginAsync(CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .IfSuccessTryFinallyAsync(
                () => AuthenticationService.IsVerifiedByLoginAsync(Login, cancellationToken)
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
                                );
                            }

                            var user = Mapper.Map<User>(this);

                            return TokenService.LoginAsync(user, cancellationToken)
                                .IfSuccessAsync(
                                    () => this.InvokeUIBackgroundAsync(() => Account.Login = user.Login)
                                        .IfSuccessAsync(
                                            () => RememberMeAsync(cancellationToken),
                                            cancellationToken
                                        )
                                        .IfSuccessAsync(
                                            () => Navigator.NavigateToAsync(
                                                ActionHelper<RootToDoItemsViewModel>.Empty,
                                                cancellationToken
                                            ),
                                            cancellationToken
                                        ),
                                    cancellationToken
                                );
                        },
                        cancellationToken
                    ),
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ToValueTask()
                    .ConfigureAwait(false),
                cancellationToken
            );
    }

    private ConfiguredValueTaskAwaitable<Result> RememberMeAsync(CancellationToken cancellationToken)
    {
        if (!IsRememberMe)
        {
            return Result.AwaitableFalse;
        }

        return TokenService.GetTokenAsync(cancellationToken)
            .IfSuccessAsync(
                token =>
                {
                    var item = new LoginStorageItem
                    {
                        Token = token,
                    };

                    return ObjectStorage.SaveObjectAsync(StorageIds.LoginId, item);
                },
                cancellationToken
            );
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        EnterCommand = CreateCommandFromTask<LoginView>(TaskWork.Create<LoginView>(EnterAsync).RunAsync);

        LoginCommand = CreateCommandFromTask(
            TaskWork.Create(LoginAsync).RunAsync,
            this.WhenAnyValue(x => x.HasErrors).Select(x => !x)
        );

        Disposables.Add(
            this.WhenAnyValue(x => x.Login)
                .Skip(1)
                .Subscribe(
                    _ =>
                    {
                        loginChanged = true;
                        this.RaisePropertyChanged(nameof(HasErrors));
                    }
                )
        );

        Disposables.Add(
            this.WhenAnyValue(x => x.Password)
                .Skip(1)
                .Subscribe(
                    _ =>
                    {
                        passwordChanged = true;
                        this.RaisePropertyChanged(nameof(HasErrors));
                    }
                )
        );

        return this.InvokeUIBackgroundAsync(() => IsBusy = true)
            .IfSuccessTryFinallyAsync(
                () =>
                {
                    if (!TryAutoLogin)
                    {
                        return Result.AwaitableFalse;
                    }

                    return ObjectStorage.GetObjectOrDefaultAsync<LoginViewModelSetting>(ViewId, cancellationToken)
                        .IfSuccessAsync(
                            setting =>
                            {
                                return SetStateAsync(setting, cancellationToken)
                                    .IfSuccessAsync(
                                        () => ObjectStorage.IsExistsAsync(StorageIds.LoginId),
                                        cancellationToken
                                    )
                                    .IfSuccessAsync(
                                        value =>
                                        {
                                            if (!value)
                                            {
                                                return Result.AwaitableFalse;
                                            }

                                            return ObjectStorage.GetObjectAsync<LoginStorageItem>(StorageIds.LoginId)
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
                                                            .IfSuccessAsync(
                                                                () => Navigator.NavigateToAsync(
                                                                    ActionHelper<RootToDoItemsViewModel>.Empty,
                                                                    cancellationToken
                                                                ),
                                                                cancellationToken
                                                            );
                                                    },
                                                    cancellationToken
                                                );
                                        },
                                        cancellationToken
                                    );
                            },
                            cancellationToken
                        );
                },
                () => this.InvokeUIBackgroundAsync(() => IsBusy = false)
                    .ToValueTask()
                    .ConfigureAwait(false),
                cancellationToken
            );
    }

    private ConfiguredValueTaskAwaitable<Result> EnterAsync(LoginView view, CancellationToken cancellationToken)
    {
        var loginTextBox = view.FindControl<TextBox>(LoginView.LoginTextBoxName);

        if (loginTextBox is null)
        {
            return Result.AwaitableFalse;
        }

        var passwordTextBox = view.FindControl<TextBox>(LoginView.PasswordTextBoxName);

        if (passwordTextBox is null)
        {
            return Result.AwaitableFalse;
        }

        if (loginTextBox.IsFocused)
        {
            passwordTextBox.Focus();

            return Result.AwaitableFalse;
        }

        if (HasErrors)
        {
            return Result.AwaitableFalse;
        }

        return this.InvokeUIBackgroundAsync(() => LoginCommand.Execute(null));
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new LoginViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<LoginViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                    () =>
                    {
                        TryAutoLogin = false;
                        Login = s.Login;
                    }
                ),
                cancellationToken
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