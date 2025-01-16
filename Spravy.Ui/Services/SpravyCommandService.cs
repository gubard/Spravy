using Spravy.Ui.Setting;

namespace Spravy.Ui.Services;

public class SpravyCommandService
{
    private readonly INavigator navigator;
    private readonly IViewFactory viewFactory;

    public SpravyCommandService(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IToDoCache toDoCache,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IAuthenticationService authenticationService,
        IPasswordService passwordService,
        IRootViewFactory rootViewFactory,
        IObjectStorage objectStorage,
        AccountNotify accountNotify,
        ITokenService tokenService,
        ISpravyNotificationManager spravyNotificationManager,
        INavigator navigator,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory,
        IToDoUiService toDoUiService,
        IScheduleService scheduleService,
        IEventUpdater eventUpdater,
        IAudioService audioService,
        Application application,
        SoundSettingsNotify soundSettingsNotify
    )
    {
        this.navigator = navigator;
        this.viewFactory = viewFactory;

        NavigateToToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoCache.GetToDoItem(item.CurrentId)
                   .IfSuccessAsync(
                        parent => navigator.NavigateToAsync(viewFactory.CreateToDoItemViewModel(parent), ct),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        NavigateToCurrentToDoItem = SpravyCommand.Create(
            ct => toDoUiService.GetRequest(GetToDo.WithDefaultItems.SetIsCurrentActiveItem(true), ct)
               .IfSuccessAsync(
                    response =>
                    {
                        if (!response.CurrentActive.TryGetValue(out var value))
                        {
                            return navigator.NavigateToAsync(viewFactory.CreateRootToDoItemsViewModel(), ct);
                        }

                        if (value.ParentId.TryGetValue(out var parentId))
                        {
                            return toDoCache.GetToDoItem(parentId)
                               .IfSuccessAsync(
                                    parent =>
                                        navigator.NavigateToAsync(viewFactory.CreateToDoItemViewModel(parent), ct),
                                    ct
                                );
                        }

                        return navigator.NavigateToAsync(viewFactory.CreateRootToDoItemsViewModel(), ct);
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Back = SpravyCommand.Create(
            ct => navigator.NavigateBackAsync(ct).ToResultOnlyAsync(),
            errorHandler,
            taskProgressService
        );

        SendNewVerificationCode = SpravyCommand.Create<IVerificationEmail>(
            (verificationEmail, ct) => verificationEmail.IdentifierType switch
            {
                UserIdentifierType.Email => authenticationService.UpdateVerificationCodeByEmailAsync(
                    verificationEmail.EmailOrLogin,
                    ct
                ),
                UserIdentifierType.Login => authenticationService.UpdateVerificationCodeByLoginAsync(
                    verificationEmail.EmailOrLogin,
                    ct
                ),
                _ => new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                   .ToValueTaskResult()
                   .ConfigureAwait(false),
            },
            errorHandler,
            taskProgressService
        );

        GeneratePassword = SpravyCommand.Create<PasswordItemEntityNotify>(
            (passwordItem, ct) => passwordService.GeneratePasswordAsync(passwordItem.Id, ct)
               .IfSuccessAsync(text => clipboardService.SetTextAsync(text, ct), ct)
               .IfSuccessAsync(
                    () => spravyNotificationManager.Show(
                        new TextLocalization("Lang.CopiedPassword", passwordItem)
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CopyLogin = SpravyCommand.Create<PasswordItemEntityNotify>(
            (passwordItem, ct) =>
                clipboardService.SetTextAsync(passwordItem.Login, ct)
                   .IfSuccessAsync(
                        () => spravyNotificationManager.Show(
                            new TextLocalization("Lang.CopiedLogin", passwordItem)
                        ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        DeletePasswordItem = SpravyCommand.Create<PasswordItemEntityNotify>(
            (passwordItem, ct) => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Content,
                viewFactory.CreateDeletePasswordItemViewModel(passwordItem),
                _ => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                   .IfSuccessAsync(() => passwordService.DeletePasswordItemAsync(passwordItem.Id, ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            ),
            errorHandler,
            taskProgressService
        );

        Complete = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) =>
            {
                uiApplicationService.StopCurrentView();

                toDoItemEditId.GetToDoItemEditId()
                   .IfSuccessAsync(
                        editId =>
                        {
                            var playComplete = editId.ResultItems.Any(x => x.IsCan == ToDoItemIsCan.CanComplete);

                            var ownerViewModel = uiApplicationService
                               .GetCurrentView<IToDoSubItemsViewModelOwner>()
                               .GetValueOrDefault();

                            return this.PostUiBackground(
                                    () => editId.ResultItems
                                       .ToResult()
                                       .IfSuccessForEach(
                                            item =>
                                            {
                                                switch (item.IsCan)
                                                {
                                                    case ToDoItemIsCan.None:
                                                        return Result.Success;
                                                    case ToDoItemIsCan.CanComplete:
                                                    case ToDoItemIsCan.CanIncomplete:
                                                    {
                                                        item.IsUpdated = false;

                                                        if (ownerViewModel is not null)
                                                        {
                                                            return ownerViewModel.ToDoSubItemsViewModel.RefreshUi();
                                                        }

                                                        return Result.Success;
                                                    }
                                                    default:
                                                        return new(new ToDoItemIsCanOutOfRangeError(item.IsCan));
                                                }
                                            }
                                        ),
                                    CancellationToken.None
                                )
                               .IfSuccessAsync(
                                    () => toDoService.SwitchCompleteAsync(
                                        editId.ResultCurrentIds,
                                        CancellationToken.None
                                    ),
                                    CancellationToken.None
                                )
                               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct)
                               .IfSuccessAsync(
                                    () =>
                                    {
                                        if (playComplete)
                                        {
                                            return audioService.PlayCompleteAsync(ct);
                                        }

                                        return Result.AwaitableSuccess;
                                    },
                                    ct
                                )
                               .IfErrorsAsync(
                                    errors => dialogViewer.ShowInfoDialogAsync(
                                        viewFactory,
                                        DialogViewLayer.Error,
                                        viewFactory.CreateErrorViewModel(errors),
                                        ct
                                    ),
                                    ct
                                );
                        },
                        CancellationToken.None
                    );

                return Result.AwaitableSuccess;
            },
            errorHandler,
            taskProgressService
        );

        AddChild = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateAddToDoItemViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Delete = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateDeleteToDoItemViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(uiApplicationService.GetCurrentViewType, ct)
                           .IfSuccessAsync(
                                type =>
                                {
                                    if (type != typeof(ToDoItemViewModel))
                                    {
                                        return Result.AwaitableSuccess;
                                    }

                                    if (editId.Item.TryGetValue(out var item) && item.Parent is not null)
                                    {
                                        return navigator.NavigateToAsync(
                                            viewFactory.CreateToDoItemViewModel(item.Parent),
                                            ct
                                        );
                                    }

                                    return navigator.NavigateToAsync(viewFactory.CreateRootToDoItemsViewModel(), ct);
                                },
                                ct
                            )
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ShowSetting = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateToDoItemSettingsViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                )
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        OpenLeaf = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) =>
                toDoItemEditId.GetToDoItemEditId()
                   .IfSuccessAsync(
                        editId => navigator.NavigateToAsync(
                            viewFactory.CreateLeafToDoItemsViewModel(editId.Item, editId.Items),
                            ct
                        ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        ChangeParent = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Input,
                        viewFactory.CreateChangeParentViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Input, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ToDoItemCopyToClipboard = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateToDoItemToStringSettingsViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RandomizeChildrenOrder = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateRandomizeChildrenOrderViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ChangeOrder = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateChangeToDoItemOrderIndexViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Reset = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateResetToDoItemViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        AddToFavorite = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => toDoService
                       .EditToDoItemsAsync(
                            new EditToDoItems().SetIds(editId.ResultCurrentIds).SetIsFavorite(new(true)),
                            ct
                        )
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RemoveFromFavorite = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => toDoService
                       .EditToDoItemsAsync(
                            new EditToDoItems().SetIds(editId.ResultCurrentIds).SetIsFavorite(new(false)),
                            ct
                        )
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        OpenLink = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => editId.ResultItems
                       .Select(x => x.Link)
                       .Where(x => !x.IsNullOrWhiteSpace())
                       .ToResult()
                       .IfSuccessForEachAsync(link => openerLink.OpenLinkAsync(link.ToUri(), ct), ct),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Clone = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Input,
                        viewFactory.CreateCloneViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Input, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CreateReference = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Input,
                        viewFactory.CreateCreateReferenceViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Input, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        SwitchPane = SpravyCommand.Create(
            ct => ct.PostUiBackground(
                    () =>
                    {
                        rootViewFactory.CreateMainSplitViewModel().IsPaneOpen = !rootViewFactory
                           .CreateMainSplitViewModel()
                           .IsPaneOpen;

                        return Result.Success;
                    },
                    ct
                )
               .GetAwaitable(),
            errorHandler,
            taskProgressService
        );

        Logout = SpravyCommand.Create(
            ct => objectStorage.IsExistsAsync(StorageIds.LoginId, ct)
               .IfSuccessAsync(
                    value =>
                    {
                        if (value)
                        {
                            return objectStorage.DeleteAsync(StorageIds.LoginId, ct);
                        }

                        accountNotify.Login = string.Empty;

                        return Result.AwaitableSuccess;
                    },
                    ct
                )
               .IfSuccessAsync(() => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct), ct)
               .IfSuccessAsync(
                    () => ct.PostUiBackground(
                        () =>
                        {
                            rootViewFactory.CreateMainSplitViewModel().IsPaneOpen = false;

                            return Result.Success;
                        },
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RefreshCurrentView = SpravyCommand.Create(
            uiApplicationService.RefreshCurrentViewAsync,
            errorHandler,
            taskProgressService
        );

        SetToDoItemDescription = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateEditDescriptionViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        AddPasswordItem = SpravyCommand.Create(
            ct => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Content,
                viewFactory.CreateAddPasswordItemViewModel(),
                vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                   .IfSuccessAsync(
                        () => passwordService.AddPasswordItemAsync(
                            vm.EditPasswordItemViewModel.ToAddPasswordOptions(),
                            ct
                        ),
                        ct
                    )
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            ),
            errorHandler,
            taskProgressService
        );

        ShowPasswordItemSetting = SpravyCommand.Create<PasswordItemEntityNotify>(
            (item, ct) => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Content,
                viewFactory.CreatePasswordItemSettingsViewModel(item),
                vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                   .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            ),
            errorHandler,
            taskProgressService
        );

        NavigateToActiveToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => toDoUiService.GetRequest(
                    GetToDo.WithDefaultItems.SetActiveItem(
                        item.Id
                    ),
                    ct
                )
               .IfSuccessAsync(
                    response =>
                    {
                        if (response.ActiveItems.Span[0].Item.TryGetValue(out var value))
                        {
                            if (value.ParentId.TryGetValue(out var parentId))
                            {
                                return toDoCache.GetToDoItem(parentId)
                                   .IfSuccessAsync(
                                        parent =>
                                            navigator.NavigateToAsync(viewFactory.CreateToDoItemViewModel(parent), ct),
                                        ct
                                    );
                            }

                            return navigator.NavigateToAsync(viewFactory.CreateRootToDoItemsViewModel(), ct);
                        }

                        return uiApplicationService.RefreshCurrentViewAsync(ct);
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ForgotPassword = SpravyCommand.Create<ForgotPasswordViewModel>(
            (vm, ct) => Result.AwaitableSuccess
               .IfSuccessAsync(
                    () => vm.IdentifierType switch
                    {
                        UserIdentifierType.Email => authenticationService.UpdatePasswordByEmailAsync(
                            vm.EmailOrLogin,
                            vm.VerificationCode.ToUpperInvariant(),
                            vm.NewPassword,
                            ct
                        ),
                        UserIdentifierType.Login => authenticationService.UpdatePasswordByLoginAsync(
                            vm.EmailOrLogin,
                            vm.VerificationCode.ToUpperInvariant(),
                            vm.NewPassword,
                            ct
                        ),
                        _ => new Result(new UserIdentifierTypeOutOfRangeError(vm.IdentifierType)).ToValueTaskResult()
                           .ConfigureAwait(false),
                    },
                    ct
                )
               .IfSuccessAsync(() => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct), ct),
            errorHandler,
            taskProgressService
        );

        CreateUserViewEnter = SpravyCommand.Create<CreateUserViewModel>(
            (vm, ct) => vm.View
               .CastObject<CreateUserView>(nameof(vm.View))
               .IfSuccessAsync(
                    view =>
                    {
                        if (view.EmailTextBox.IsFocused)
                        {
                            return this.InvokeUiAsync(
                                () =>
                                {
                                    view.LoginTextBox.Focus();

                                    return Result.Success;
                                }
                            );
                        }

                        if (view.LoginTextBox.IsFocused)
                        {
                            return this.InvokeUiAsync(
                                () =>
                                {
                                    view.PasswordTextBox.Focus();

                                    return Result.Success;
                                }
                            );
                        }

                        if (view.PasswordTextBox.IsFocused)
                        {
                            return this.InvokeUiAsync(
                                () =>
                                {
                                    view.RepeatPasswordTextBox.Focus();

                                    return Result.Success;
                                }
                            );
                        }

                        if (vm.HasErrors)
                        {
                            return Result.AwaitableSuccess;
                        }

                        return CreateUserAsync(vm, authenticationService, ct);
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CreateUser = SpravyCommand.Create<CreateUserViewModel>(
            (vm, ct) => CreateUserAsync(vm, authenticationService, ct),
            errorHandler,
            taskProgressService
        );

        LoginViewInitialized = SpravyCommand.Create<LoginView>(
            (view, ct) =>
            {
                eventUpdater.Stop();

                return Result.AwaitableSuccess.IfSuccessTryFinallyAsync(
                    () => objectStorage.IsExistsAsync(StorageIds.LoginId, ct)
                       .IfSuccessAsync(
                            value =>
                            {
                                if (!value)
                                {
                                    return this.PostUiBackground(() => view.FocusTextBoxUi("LoginTextBox"), ct)
                                       .ToValueTaskResult()
                                       .ConfigureAwait(false);
                                }

                                return objectStorage.GetObjectAsync<LoginStorageItem>(StorageIds.LoginId, ct)
                                   .IfSuccessAsync(
                                        item =>
                                        {
                                            var jwtHandler = new JwtSecurityTokenHandler();
                                            var token = item.Token;
                                            var jwtToken = jwtHandler.ReadJwtToken(token);
                                            accountNotify.Login = jwtToken.Claims.GetName();

                                            return tokenService.LoginAsync(item.Token.ThrowIfNullOrWhiteSpace(), ct)
                                               .IfSuccessAsync(
                                                    () => toDoUiService.GetRequest(
                                                        GetToDo.WithDefaultItems.SetIsSelectorItems(true),
                                                        ct
                                                    ),
                                                    ct
                                                )
                                               .IfSuccessAsync(
                                                    _ => navigator.NavigateToAsync(
                                                        viewFactory.CreateRootToDoItemsViewModel(),
                                                        ct
                                                    ),
                                                    ct
                                                )
                                               .IfSuccessAsync(
                                                    () =>
                                                    {
                                                        eventUpdater.Start();

                                                        return Result.AwaitableSuccess;
                                                    },
                                                    ct
                                                );
                                        },
                                        ct
                                    );
                            },
                            ct
                        ),
                    () => this.PostUiBackground(
                            () =>
                            {
                                view.ViewModel.IsBusy = false;

                                return Result.Success;
                            },
                            ct
                        )
                       .GetAwaitable(),
                    ct
                );
            },
            errorHandler,
            taskProgressService
        );

        Login = SpravyCommand.Create<LoginViewModel>(
            (vm, ct) => LoginAsync(
                    vm,
                    authenticationService,
                    tokenService,
                    objectStorage,
                    toDoUiService,
                    accountNotify,
                    uiApplicationService,
                    ct
                )
               .IfSuccessAsync(
                    () =>
                    {
                        eventUpdater.Start();

                        return Result.AwaitableSuccess;
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        LoginViewEnter = SpravyCommand.Create<LoginViewModel>(
            (vm, ct) => vm.View
               .CastObject<LoginView>(nameof(vm.View))
               .IfSuccessAsync(
                    view =>
                    {
                        if (this.GetUiValue(() => view.LoginTextBox.IsFocused))
                        {
                            return this.InvokeUiAsync(
                                () =>
                                {
                                    view.PasswordTextBox.FocusTextBoxUi();

                                    return Result.Success;
                                }
                            );
                        }

                        if (this.GetUiValue(() => view.PasswordTextBox.IsFocused))
                        {
                            if (view.ViewModel.HasErrors)
                            {
                                return Result.AwaitableSuccess;
                            }

                            return LoginAsync(
                                view.ViewModel,
                                authenticationService,
                                tokenService,
                                objectStorage,
                                toDoUiService,
                                accountNotify,
                                uiApplicationService,
                                ct
                            );
                        }

                        return Result.AwaitableSuccess;
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CopyToClipboard = SpravyCommand.Create<string>(
            (str, ct) =>
                clipboardService.SetTextAsync(str, ct)
                   .IfSuccessAsync(
                        () => spravyNotificationManager.Show(new TextLocalization("Lang.CopyToClipboard")),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        NavigateToRootToDoItems = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateRootToDoItemsViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToTodayToDoItems = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateTodayToDoItemsViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToSearchToDoItems = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateSearchToDoItemsViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToPasswordGenerator = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreatePasswordGeneratorViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToSetting = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateSettingViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToCreateUser = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateCreateUserViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToEmailOrLoginInput = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateEmailOrLoginInputViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        UpdateEmail = SpravyCommand.Create<IVerificationEmail>(
            (verificationEmail, ct) => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Input,
                viewFactory.CreateTextViewModel(),
                text => verificationEmail.IdentifierType switch
                {
                    UserIdentifierType.Email => authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
                        verificationEmail.EmailOrLogin,
                        text.Text,
                        ct
                    ),
                    UserIdentifierType.Login => authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
                        verificationEmail.EmailOrLogin,
                        text.Text,
                        ct
                    ),
                    _ => new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                       .ToValueTaskResult()
                       .ConfigureAwait(false),
                },
                ct
            ),
            errorHandler,
            taskProgressService
        );

        VerificationEmail = SpravyCommand.Create<IVerificationEmail>(
            (verificationEmail, ct) => verificationEmail.IdentifierType switch
            {
                UserIdentifierType.Email => authenticationService
                   .VerifiedEmailByEmailAsync(
                        verificationEmail.EmailOrLogin,
                        verificationEmail.VerificationCode.ToUpperInvariant(),
                        ct
                    )
                   .IfSuccessAsync(() => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct), ct),
                UserIdentifierType.Login => authenticationService
                   .VerifiedEmailByLoginAsync(
                        verificationEmail.EmailOrLogin,
                        verificationEmail.VerificationCode.ToUpperInvariant(),
                        ct
                    )
                   .IfSuccessAsync(() => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct), ct),
                _ => new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                   .ToValueTaskResult()
                   .ConfigureAwait(false),
            },
            errorHandler,
            taskProgressService
        );

        MainSplitViewModelInitialized = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToTimers = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreateTimersViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToPolicy = SpravyCommand.Create(
            ct => navigator.NavigateToAsync(viewFactory.CreatePolicyViewModel(), ct),
            errorHandler,
            taskProgressService
        );

        AddTimer = SpravyCommand.Create(
            ct => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Content,
                viewFactory.CreateAddTimerViewModel(),
                vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                   .IfSuccessAsync(() => vm.ApplySettingsAsync(ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            ),
            errorHandler,
            taskProgressService
        );

        DeleteTimer = SpravyCommand.Create<TimerItemNotify>(
            (item, ct) => dialogViewer.ShowConfirmDialogAsync(
                viewFactory,
                DialogViewLayer.Content,
                viewFactory.CreateDeleteTimerViewModel(item),
                vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                   .IfSuccessAsync(() => scheduleService.RemoveTimerAsync(vm.Item.Id, ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                ct
            ),
            errorHandler,
            taskProgressService
        );

        CreateTimer = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => dialogViewer.ShowConfirmDialogAsync(
                        viewFactory,
                        DialogViewLayer.Content,
                        viewFactory.CreateToDoItemCreateTimerViewModel(editId.Item, editId.Items),
                        vm => dialogViewer.CloseDialogAsync(DialogViewLayer.Content, ct)
                           .IfSuccessAsync(() => vm.CreateAddTimerParametersAsync(ct), ct)
                           .IfSuccessAsync(parameters => scheduleService.AddTimerAsync(parameters, ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                        ct
                    ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        MainViewInitialized = SpravyCommand.Create(
            ct =>
            {
                var key = TypeCache<SettingViewModel>.Name;

                return objectStorage.IsExistsAsync(key, ct)
                   .IfSuccessAsync(
                        isExists =>
                        {
                            if (isExists)
                            {
                                return objectStorage.GetObjectAsync<AppSetting>(key, ct)
                                   .IfSuccessAsync(
                                        setting => setting.PostUiBackground(
                                            () =>
                                            {
                                                soundSettingsNotify.IsMute = setting.IsMute;
                                                application.RequestedThemeVariant = setting.Theme.ToThemeVariant();
                                                var lang = application.GetLang(setting.Language);
                                                application.Resources.MergedDictionaries.Remove(lang);
                                                application.Resources.MergedDictionaries.Add(lang);

                                                return Result.Success;
                                            },
                                            ct
                                        ),
                                        ct
                                    );
                            }

                            return Result.AwaitableSuccess;
                        },
                        ct
                    );
            },
            errorHandler,
            taskProgressService
        );

        AddToBookmark = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => toDoService
                       .EditToDoItemsAsync(
                            new EditToDoItems().SetIds(editId.ResultCurrentIds).SetIsBookmark(new(true)),
                            ct
                        )
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RemoveFromBookmark = SpravyCommand.Create<IToDoItemEditId>(
            (toDoItemEditId, ct) => toDoItemEditId.GetToDoItemEditId()
               .IfSuccessAsync(
                    editId => toDoService
                       .EditToDoItemsAsync(
                            new EditToDoItems().SetIds(editId.ResultCurrentIds).SetIsBookmark(new(false)),
                            ct
                        )
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
                    ct
                ),
            errorHandler,
            taskProgressService
        );
    }

    public SpravyCommand CopyLogin { get; }
    public SpravyCommand CopyToClipboard { get; }
    public SpravyCommand Complete { get; }
    public SpravyCommand CreateTimer { get; }
    public SpravyCommand AddToFavorite { get; }
    public SpravyCommand RemoveFromFavorite { get; }
    public SpravyCommand OpenLink { get; }
    public SpravyCommand AddChild { get; }
    public SpravyCommand Delete { get; }
    public SpravyCommand ShowSetting { get; }
    public SpravyCommand OpenLeaf { get; }
    public SpravyCommand ChangeParent { get; }
    public SpravyCommand ToDoItemCopyToClipboard { get; }
    public SpravyCommand RandomizeChildrenOrder { get; }
    public SpravyCommand ChangeOrder { get; }
    public SpravyCommand Reset { get; }
    public SpravyCommand Clone { get; }
    public SpravyCommand CreateReference { get; }
    public SpravyCommand NavigateToToDoItem { get; }
    public SpravyCommand NavigateToActiveToDoItem { get; }
    public SpravyCommand AddToBookmark { get; }
    public SpravyCommand RemoveFromBookmark { get; }

    public SpravyCommand GeneratePassword { get; }
    public SpravyCommand DeletePasswordItem { get; }
    public SpravyCommand AddPasswordItem { get; }
    public SpravyCommand ShowPasswordItemSetting { get; }

    public SpravyCommand SwitchPane { get; }
    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand NavigateToCurrentToDoItem { get; }
    public SpravyCommand Logout { get; }
    public SpravyCommand RefreshCurrentView { get; }
    public SpravyCommand SetToDoItemDescription { get; }

    public SpravyCommand LoginViewInitialized { get; }

    public SpravyCommand ForgotPassword { get; }

    public SpravyCommand CreateUserViewEnter { get; }
    public SpravyCommand CreateUser { get; }

    public SpravyCommand Login { get; }
    public SpravyCommand LoginViewEnter { get; }

    public SpravyCommand NavigateToRootToDoItems { get; }
    public SpravyCommand NavigateToTodayToDoItems { get; }
    public SpravyCommand NavigateToSearchToDoItems { get; }
    public SpravyCommand NavigateToTimers { get; }
    public SpravyCommand AddTimer { get; }
    public SpravyCommand DeleteTimer { get; }
    public SpravyCommand NavigateToPasswordGenerator { get; }
    public SpravyCommand NavigateToSetting { get; }
    public SpravyCommand NavigateToPolicy { get; }
    public SpravyCommand NavigateToCreateUser { get; }
    public SpravyCommand NavigateToEmailOrLoginInput { get; }

    public SpravyCommand MainSplitViewModelInitialized { get; }
    public SpravyCommand MainViewInitialized { get; }
    public SpravyCommand UpdateEmail { get; }
    public SpravyCommand VerificationEmail { get; }

    private Cvtar CreateUserAsync(
        CreateUserViewModel viewModel,
        IAuthenticationService authenticationService,
        CancellationToken ct
    )
    {
        if (viewModel.HasErrors)
        {
            return Result.AwaitableSuccess;
        }

        return this.PostUiBackground(
                () =>
                {
                    viewModel.IsBusy = true;

                    return Result.Success;
                },
                ct
            )
           .IfSuccessTryFinallyAsync(
                () => authenticationService.CreateUserAsync(viewModel.ToCreateUserOptions(), ct)
                   .IfSuccessAsync(
                        () => navigator.NavigateToAsync(
                            viewFactory.CreateVerificationCodeViewModel(viewModel.Email, UserIdentifierType.Email),
                            ct
                        ),
                        ct
                    ),
                () => this.PostUiBackground(
                        () =>
                        {
                            viewModel.IsBusy = false;

                            return Result.Success;
                        },
                        ct
                    )
                   .GetAwaitable(),
                ct
            );
    }

    private Cvtar LoginAsync(
        LoginViewModel viewModel,
        IAuthenticationService authenticationService,
        ITokenService tokenService,
        IObjectStorage objectStorage,
        IToDoUiService toDoUiService,
        AccountNotify accountNotify,
        IUiApplicationService uiApplicationService,
        CancellationToken ct
    )
    {
        if (viewModel.HasErrors)
        {
            return Result.AwaitableSuccess;
        }

        return this.PostUiBackground(
                () =>
                {
                    viewModel.IsBusy = true;

                    return Result.Success;
                },
                ct
            )
           .IfSuccessTryFinallyAsync(
                () => authenticationService.IsVerifiedByLoginAsync(viewModel.Login, ct)
                   .IfSuccessAsync(
                        isVerified =>
                        {
                            if (!isVerified)
                            {
                                return navigator.NavigateToAsync(
                                    viewFactory.CreateVerificationCodeViewModel(
                                        viewModel.Login,
                                        UserIdentifierType.Login
                                    ),
                                    ct
                                );
                            }

                            return tokenService.LoginAsync(viewModel.ToUser(), ct)
                               .IfSuccessAsync(
                                    () => this.PostUiBackground(
                                            () =>
                                            {
                                                accountNotify.Login = viewModel.Login;

                                                return Result.Success;
                                            },
                                            ct
                                        )
                                       .IfSuccessAsync(
                                            () => RememberMeAsync(viewModel, tokenService, objectStorage, ct),
                                            ct
                                        )
                                       .IfSuccessAsync(
                                            () => toDoUiService.GetRequest(
                                                GetToDo.WithDefaultItems.SetIsSelectorItems(true),
                                                ct
                                            ),
                                            ct
                                        )
                                       .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(ct), ct)
                                       .IfSuccessAsync(
                                            () => navigator.NavigateToAsync(
                                                viewFactory.CreateRootToDoItemsViewModel(),
                                                ct
                                            ),
                                            ct
                                        ),
                                    ct
                                );
                        },
                        ct
                    ),
                () => this.PostUiBackground(
                        () =>
                        {
                            viewModel.IsBusy = false;

                            return Result.Success;
                        },
                        ct
                    )
                   .GetAwaitable(),
                ct
            );
    }

    private Cvtar RememberMeAsync(
        LoginViewModel viewModel,
        ITokenService tokenService,
        IObjectStorage objectStorage,
        CancellationToken ct
    )
    {
        if (!viewModel.IsRememberMe)
        {
            return Result.AwaitableSuccess;
        }

        return tokenService.GetTokenAsync(ct)
           .IfSuccessAsync(
                token =>
                {
                    var item = new LoginStorageItem
                    {
                        Token = token,
                    };

                    return objectStorage.SaveObjectAsync(StorageIds.LoginId, item, ct);
                },
                ct
            );
    }
}