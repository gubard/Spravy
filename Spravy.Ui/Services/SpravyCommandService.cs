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
        Application application
    )
    {
        this.navigator = navigator;
        this.viewFactory = viewFactory;

        NavigateToToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoCache
                    .GetToDoItem(item.CurrentId)
                    .IfSuccessAsync(
                        parent =>
                            navigator.NavigateToAsync(
                                viewFactory.CreateToDoItemViewModel(parent),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiCreateReference = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (property, ct) =>
                property
                    .GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService
                                                    .RunProgressAsync(
                                                        selected,
                                                        i =>
                                                            toDoService
                                                                .CloneToDoItemAsync(
                                                                    i.Id,
                                                                    (
                                                                        vm.SelectedItem?.Id
                                                                    ).ToOption(),
                                                                    ct
                                                                )
                                                                .IfSuccessAsync(
                                                                    id =>
                                                                        toDoService
                                                                            .UpdateToDoItemTypeAsync(
                                                                                id,
                                                                                ToDoItemType.Reference,
                                                                                ct
                                                                            )
                                                                            .IfSuccessAsync(
                                                                                () =>
                                                                                    toDoService.UpdateReferenceToDoItemAsync(
                                                                                        id,
                                                                                        i.Id,
                                                                                        ct
                                                                                    ),
                                                                                ct
                                                                            ),
                                                                    ct
                                                                ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            uiApplicationService.RefreshCurrentViewAsync(
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiClone = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (property, ct) =>
                property
                    .GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selectedIds =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService
                                                    .RunProgressAsync(
                                                        selectedIds,
                                                        i =>
                                                            toDoService.CloneToDoItemAsync(
                                                                i,
                                                                (vm.SelectedItem?.Id).ToOption(),
                                                                ct
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        _ =>
                                                            uiApplicationService.RefreshCurrentViewAsync(
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiReset = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                toDoUiService
                    .UpdateItemAsync(view.Item, ct)
                    .IfSuccessAsync(
                        () =>
                            view.GetSelectedItems()
                                .IfSuccess(selected => selected.Select(x => x.Id).ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateResetToDoItemViewModel(view.Item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.ResetToDoItemAsync(
                                                            new(
                                                                i,
                                                                vm.IsCompleteChildrenTask,
                                                                vm.IsMoveCircleOrderIndex,
                                                                vm.IsOnlyCompletedTasks,
                                                                vm.IsCompleteCurrentTask
                                                            ),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiChangeOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            view.GetNotSelectedItems()
                                .IfSuccessAsync(
                                    noSelected =>
                                        dialogViewer.ShowConfirmDialogAsync(
                                            viewFactory,
                                            DialogViewLayer.Content,
                                            viewFactory.CreateChangeToDoItemOrderIndexViewModel(
                                                noSelected
                                            ),
                                            vm =>
                                                dialogViewer
                                                    .CloseDialogAsync(DialogViewLayer.Content, ct)
                                                    .IfSuccessAsync(
                                                        () =>
                                                            taskProgressService.RunProgressAsync(
                                                                selected,
                                                                i =>
                                                                    vm.SelectedItem.IfNotNull(
                                                                            nameof(vm.SelectedItem)
                                                                        )
                                                                        .IfSuccessAsync(
                                                                            selectedItem =>
                                                                                toDoService.UpdateToDoItemOrderIndexAsync(
                                                                                    new(
                                                                                        i.Id,
                                                                                        selectedItem.Id,
                                                                                        vm.IsAfter
                                                                                    ),
                                                                                    ct
                                                                                ),
                                                                            ct
                                                                        ),
                                                                ct
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            uiApplicationService.RefreshCurrentViewAsync(
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                    ct
                                ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiRandomizeChildrenOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateRandomizeChildrenOrderViewModel(selected),
                                _ =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.RandomizeChildrenOrderIndexAsync(
                                                            i.Id,
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiCopyToClipboard = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateToDoItemToStringSettingsViewModel(view.Item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService
                                                    .RunProgressAsync(
                                                        selected,
                                                        i =>
                                                            toDoService.ToDoItemToStringAsync(
                                                                new(
                                                                    vm.Statuses.Where(x =>
                                                                            x.IsChecked
                                                                        )
                                                                        .Select(x => x.Item)
                                                                        .ToArray(),
                                                                    i
                                                                ),
                                                                ct
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        items =>
                                                            clipboardService.SetTextAsync(
                                                                items
                                                                    .Span.Join(Environment.NewLine)
                                                                    .ToString()
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiMakeAsRoot = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.ToDoItemToRootAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiChangeParent = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(view.Item, selected),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                vm.SelectedItem.IfNotNull(nameof(vm.SelectedItem)),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            selectedItem =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.UpdateToDoItemParentAsync(
                                                            i.Id,
                                                            selectedItem.Id,
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiOpenLeaf = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            navigator.NavigateToAsync(
                                viewFactory.CreateLeafToDoItemsViewModel(selected),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiShowSetting = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateMultiToDoItemSettingViewModel(),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    item =>
                                                        Result
                                                            .AwaitableSuccess.IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsLink)
                                                                    {
                                                                        return toDoService.UpdateToDoItemLinkAsync(
                                                                            item.Id,
                                                                            vm.Link.ToOptionUri(),
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsName)
                                                                    {
                                                                        return toDoService.UpdateToDoItemNameAsync(
                                                                            item.Id,
                                                                            vm.Name,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsDueDate)
                                                                    {
                                                                        return toDoService.UpdateToDoItemDueDateAsync(
                                                                            item.Id,
                                                                            vm.DueDate,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsType)
                                                                    {
                                                                        return toDoService.UpdateToDoItemTypeAsync(
                                                                            item.Id,
                                                                            vm.Type,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiDelete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateDeleteToDoItemViewModel(view.Item, selected),
                                _ =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    item =>
                                                        toDoService.DeleteToDoItemAsync(
                                                            item.Id,
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiAddChild = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateAddToDoItemViewModel(view.Item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    item =>
                                                        vm.ConverterToAddToDoItemOptions(item.Id)
                                                            .IfSuccessAsync(
                                                                options =>
                                                                    toDoService
                                                                        .AddToDoItemAsync(
                                                                            options,
                                                                            ct
                                                                        )
                                                                        .IfSuccessAsync(
                                                                            _ =>
                                                                                uiApplicationService.RefreshCurrentViewAsync(
                                                                                    ct
                                                                                ),
                                                                            ct
                                                                        ),
                                                                ct
                                                            ),
                                                    ct
                                                ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiOpenLink = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        items =>
                            taskProgressService.RunProgressAsync(
                                items,
                                i =>
                                    i.Link.IfNotNull(nameof(i.Link))
                                        .IfSuccessAsync(
                                            link => openerLink.OpenLinkAsync(link.ToUri(), ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiRemoveFromFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.RemoveFavoriteToDoItemAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiAddToFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.AddFavoriteToDoItemAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiComplete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected,
                                i =>
                                {
                                    switch (i.IsCan)
                                    {
                                        case ToDoItemIsCan.None:
                                            return Result.AwaitableSuccess;
                                        case ToDoItemIsCan.CanComplete:
                                            return toDoService.UpdateToDoItemCompleteStatusAsync(
                                                i.Id,
                                                true,
                                                ct
                                            );
                                        case ToDoItemIsCan.CanIncomplete:
                                            return toDoService.UpdateToDoItemCompleteStatusAsync(
                                                i.Id,
                                                false,
                                                ct
                                            );
                                        default:
                                            return new Result(
                                                new ToDoItemIsCanOutOfRangeError(i.IsCan)
                                            )
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false);
                                    }
                                },
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        NavigateToCurrentToDoItem = SpravyCommand.Create(
            ct =>
                toDoService
                    .GetCurrentActiveToDoItemAsync(ct)
                    .IfSuccessAsync(
                        activeToDoItem =>
                        {
                            if (activeToDoItem.TryGetValue(out var value))
                            {
                                if (value.ParentId.TryGetValue(out var parentId))
                                {
                                    return toDoCache
                                        .GetToDoItem(parentId)
                                        .IfSuccessAsync(
                                            parent =>
                                                navigator.NavigateToAsync(
                                                    viewFactory.CreateToDoItemViewModel(parent),
                                                    ct
                                                ),
                                            ct
                                        );
                                }
                            }

                            return navigator.NavigateToAsync(
                                viewFactory.CreateRootToDoItemsViewModel(),
                                ct
                            );
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
            (verificationEmail, ct) =>
                verificationEmail.IdentifierType switch
                {
                    UserIdentifierType.Email
                        => authenticationService.UpdateVerificationCodeByEmailAsync(
                            verificationEmail.EmailOrLogin,
                            ct
                        ),
                    UserIdentifierType.Login
                        => authenticationService.UpdateVerificationCodeByLoginAsync(
                            verificationEmail.EmailOrLogin,
                            ct
                        ),
                    _
                        => new Result(
                            new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType)
                        )
                            .ToValueTaskResult()
                            .ConfigureAwait(false),
                },
            errorHandler,
            taskProgressService
        );

        GeneratePassword = SpravyCommand.Create<PasswordItemEntityNotify>(
            (passwordItem, ct) =>
                passwordService
                    .GeneratePasswordAsync(passwordItem.Id, ct)
                    .IfSuccessAsync(clipboardService.SetTextAsync, ct)
                    .IfSuccessAsync(
                        () =>
                            spravyNotificationManager.ShowAsync(
                                new TextLocalization(
                                    "PasswordGeneratorView.Notification.CopyPassword",
                                    passwordItem
                                ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        DeletePasswordItem = SpravyCommand.Create<PasswordItemEntityNotify>(
            (passwordItem, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateDeletePasswordItemViewModel(passwordItem),
                    _ =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () => passwordService.DeletePasswordItemAsync(passwordItem.Id, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Complete = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
            {
                uiApplicationService.StopCurrentView();

                Result
                    .AwaitableSuccess.IfSuccessAsync(
                        () =>
                            item.IsCan switch
                            {
                                ToDoItemIsCan.None => Result.AwaitableSuccess,
                                ToDoItemIsCan.CanComplete
                                    => this.PostUi(() =>
                                        {
                                            item.IsCan = ToDoItemIsCan.None;
                                            item.Status = ToDoItemStatus.Completed;

                                            return uiApplicationService
                                                .GetCurrentView<IToDoItemsView>()
                                                .IfSuccess(x => x.RemoveUi(item));
                                        })
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemCompleteStatusAsync(
                                                    item.CurrentId,
                                                    true,
                                                    ct
                                                ),
                                            ct
                                        ),
                                ToDoItemIsCan.CanIncomplete
                                    => this.PostUi(() =>
                                        {
                                            item.IsCan = ToDoItemIsCan.None;
                                            item.Status = ToDoItemStatus.ReadyForComplete;

                                            return uiApplicationService
                                                .GetCurrentView<IToDoItemsView>()
                                                .IfSuccess(x => x.RemoveUi(item));
                                        })
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemCompleteStatusAsync(
                                                    item.CurrentId,
                                                    false,
                                                    ct
                                                ),
                                            ct
                                        ),
                                _
                                    => new Result(new ToDoItemIsCanOutOfRangeError(item.IsCan))
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false),
                            },
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct);

                return Result.AwaitableSuccess;
            },
            errorHandler,
            taskProgressService
        );

        AddChild = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateAddToDoItemViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    vm.ConverterToAddToDoItemOptions()
                                        .IfSuccessAsync(
                                            options =>
                                                toDoService
                                                    .AddToDoItemAsync(options, ct)
                                                    .IfSuccessAsync(
                                                        _ =>
                                                            uiApplicationService.RefreshCurrentViewAsync(
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Delete = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateDeleteToDoItemViewModel(item),
                    _ =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(item.Id, ct), ct)
                            .IfSuccessAsync(uiApplicationService.GetCurrentViewType, ct)
                            .IfSuccessAsync(
                                type =>
                                {
                                    if (type != typeof(ToDoItemViewModel))
                                    {
                                        return Result.AwaitableSuccess;
                                    }

                                    if (item.Parent is null)
                                    {
                                        return navigator.NavigateToAsync(
                                            viewFactory.CreateRootToDoItemsViewModel(),
                                            ct
                                        );
                                    }

                                    return navigator.NavigateToAsync(
                                        viewFactory.CreateToDoItemViewModel(item.Parent),
                                        ct
                                    );
                                },
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ShowSetting = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoUiService
                    .UpdateItemAsync(item, ct)
                    .IfSuccessAsync(
                        () =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateToDoItemSettingsViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemNameAsync(
                                                    item.Id,
                                                    vm.ToDoItemContent.Name,
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemLinkAsync(
                                                    item.Id,
                                                    vm.ToDoItemContent.Link.ToOptionUri(),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemTypeAsync(
                                                    item.Id,
                                                    vm.ToDoItemContent.Type,
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => vm.Settings.ApplySettingsAsync(ct),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => toDoUiService.UpdateItemAsync(item, ct),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        OpenLeaf = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                navigator.NavigateToAsync(viewFactory.CreateLeafToDoItemsViewModel(item), ct),
            errorHandler,
            taskProgressService
        );

        ChangeParent = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Input,
                    viewFactory.CreateToDoItemSelectorViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Input, ct)
                            .IfSuccessAsync(
                                () => vm.SelectedItem.IfNotNull(nameof(vm.SelectedItem)),
                                ct
                            )
                            .IfSuccessAsync(
                                selectedItem =>
                                    toDoService.UpdateToDoItemParentAsync(
                                        item.Id,
                                        selectedItem.Id,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        MakeAsRoot = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoService
                    .ToDoItemToRootAsync(item.Id, ct)
                    .IfSuccessAsync(
                        () =>
                            navigator.NavigateToAsync(
                                viewFactory.CreateRootToDoItemsViewModel(),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        ToDoItemCopyToClipboard = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateToDoItemToStringSettingsViewModel(item),
                    vm =>
                    {
                        var statuses = vm
                            .Statuses.Where(x => x.IsChecked)
                            .Select(x => x.Item)
                            .ToArray();

                        var options = new ToDoItemToStringOptions(statuses, item.CurrentId);

                        return dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService
                                        .ToDoItemToStringAsync(options, ct)
                                        .IfSuccessAsync(clipboardService.SetTextAsync, ct),
                                ct
                            );
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RandomizeChildrenOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateRandomizeChildrenOrderViewModel(item),
                    _ =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.RandomizeChildrenOrderIndexAsync(
                                        item.CurrentId,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ChangeOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateChangeToDoItemOrderIndexViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () => vm.SelectedItem.IfNotNull(nameof(vm.SelectedItem)),
                                ct
                            )
                            .IfSuccessAsync(
                                selectedItem =>
                                {
                                    var options = new UpdateOrderIndexToDoItemOptions(
                                        item.Id,
                                        selectedItem.Id,
                                        vm.IsAfter
                                    );

                                    return dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.UpdateToDoItemOrderIndexAsync(
                                                    options,
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        );
                                },
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Reset = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoUiService
                    .UpdateItemAsync(item, ct)
                    .IfSuccessAsync(
                        () =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateResetToDoItemViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                toDoService.ResetToDoItemAsync(
                                                    vm.ToResetToDoItemOptions(),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        AddToFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoService
                    .AddFavoriteToDoItemAsync(item.Id, ct)
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        RemoveFromFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoService
                    .RemoveFavoriteToDoItemAsync(item.Id, ct)
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        OpenLink = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
            {
                var link = item.Link.ThrowIfNull().ToUri();

                return openerLink.OpenLinkAsync(link, ct);
            },
            errorHandler,
            taskProgressService
        );

        Clone = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Input,
                    viewFactory.CreateToDoItemSelectorViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Input, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.CloneToDoItemAsync(
                                        item.Id,
                                        (vm.SelectedItem?.Id).ToOption(),
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CreateReference = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Input,
                    viewFactory.CreateToDoItemSelectorViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Input, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService
                                        .CloneToDoItemAsync(
                                            item.Id,
                                            (vm.SelectedItem?.Id).ToOption(),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            id =>
                                                toDoService
                                                    .UpdateToDoItemTypeAsync(
                                                        id,
                                                        ToDoItemType.Reference,
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            toDoService.UpdateReferenceToDoItemAsync(
                                                                id,
                                                                item.Id,
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        MultiAddChildToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateAddToDoItemViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        vm.ConverterToAddToDoItemOptions(i.Id)
                                                            .IfSuccessAsync(
                                                                options =>
                                                                    toDoService
                                                                        .AddToDoItemAsync(
                                                                            options,
                                                                            ct
                                                                        )
                                                                        .IfSuccessAsync(
                                                                            _ =>
                                                                                uiApplicationService.RefreshCurrentViewAsync(
                                                                                    ct
                                                                                ),
                                                                            ct
                                                                        ),
                                                                ct
                                                            ),
                                                    ct
                                                ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiShowSettingToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateMultiToDoItemSettingViewModel(),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        Result
                                                            .AwaitableSuccess.IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsLink)
                                                                    {
                                                                        return toDoService.UpdateToDoItemLinkAsync(
                                                                            i.Id,
                                                                            vm.Link.ToOptionUri(),
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsName)
                                                                    {
                                                                        return toDoService.UpdateToDoItemNameAsync(
                                                                            i.Id,
                                                                            vm.Name,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsDueDate)
                                                                    {
                                                                        return toDoService.UpdateToDoItemDueDateAsync(
                                                                            i.Id,
                                                                            vm.DueDate,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                () =>
                                                                {
                                                                    if (vm.IsType)
                                                                    {
                                                                        return toDoService.UpdateToDoItemTypeAsync(
                                                                            i.Id,
                                                                            vm.Type,
                                                                            ct
                                                                        );
                                                                    }

                                                                    return Result.AwaitableSuccess;
                                                                },
                                                                ct
                                                            ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiDeleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateDeleteToDoItemViewModel(item, selected),
                                _ =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i => toDoService.DeleteToDoItemAsync(i.Id, ct),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiOpenLeafToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            navigator.NavigateToAsync(
                                viewFactory.CreateLeafToDoItemsViewModel(item, selected),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiChangeParentToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(item, selected),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                vm.SelectedItem.IfNotNull(nameof(vm.SelectedItem)),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            selectedItem =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.UpdateToDoItemParentAsync(
                                                            i.Id,
                                                            selectedItem.Id,
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiMakeAsRootToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.ToDoItemToRootAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiCopyToClipboardToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateToDoItemToStringSettingsViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                vm
                                                    .Statuses.Where(x => x.IsChecked)
                                                    .Select(x => x.Item)
                                                    .ToArray()
                                                    .ToReadOnlyMemory()
                                                    .ToResult(),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            statuses =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.ToDoItemToStringAsync(
                                                            new(statuses, i.Id),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            items =>
                                                clipboardService.SetTextAsync(
                                                    items.Span.Join(Environment.NewLine).ToString()
                                                ),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiRandomizeChildrenOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateRandomizeChildrenOrderViewModel(item),
                                _ =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.RandomizeChildrenOrderIndexAsync(
                                                            i.Id,
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiChangeOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            item.GetNotSelectedItems()
                                .IfSuccessAsync(
                                    noSelected =>
                                        dialogViewer.ShowConfirmDialogAsync(
                                            viewFactory,
                                            DialogViewLayer.Content,
                                            viewFactory.CreateChangeToDoItemOrderIndexViewModel(
                                                item,
                                                noSelected
                                            ),
                                            vm =>
                                                dialogViewer
                                                    .CloseDialogAsync(DialogViewLayer.Content, ct)
                                                    .IfSuccessAsync(
                                                        () =>
                                                            vm.SelectedItem.IfNotNull(
                                                                nameof(vm.SelectedItem)
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        selectedItem =>
                                                            taskProgressService.RunProgressAsync(
                                                                selected,
                                                                i =>
                                                                    toDoService.UpdateToDoItemOrderIndexAsync(
                                                                        new(
                                                                            i.Id,
                                                                            selectedItem.Id,
                                                                            vm.IsAfter
                                                                        ),
                                                                        ct
                                                                    ),
                                                                ct
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            uiApplicationService.RefreshCurrentViewAsync(
                                                                ct
                                                            ),
                                                        ct
                                                    ),
                                            ct
                                        ),
                                    ct
                                ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiResetToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoUiService
                    .UpdateItemAsync(item, ct)
                    .IfSuccessAsync(
                        () =>
                            item.GetSelectedItems()
                                .IfSuccess(selected => selected.Select(x => x.Id).ToResult()),
                        ct
                    )
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateResetToDoItemViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.ResetToDoItemAsync(
                                                            new(
                                                                i,
                                                                vm.IsCompleteChildrenTask,
                                                                vm.IsMoveCircleOrderIndex,
                                                                vm.IsOnlyCompletedTasks,
                                                                vm.IsCompleteCurrentTask
                                                            ),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiCloneToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.CloneToDoItemAsync(
                                                            i,
                                                            (vm.SelectedItem?.Id).ToOption(),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiCreateReferenceToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Input,
                                viewFactory.CreateToDoItemSelectorViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Input, ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService
                                                            .CloneToDoItemAsync(
                                                                i,
                                                                (vm.SelectedItem?.Id).ToOption(),
                                                                ct
                                                            )
                                                            .IfSuccessAsync(
                                                                id =>
                                                                    toDoService
                                                                        .UpdateToDoItemTypeAsync(
                                                                            id,
                                                                            ToDoItemType.Reference,
                                                                            ct
                                                                        )
                                                                        .IfSuccessAsync(
                                                                            () =>
                                                                                toDoService.UpdateReferenceToDoItemAsync(
                                                                                    id,
                                                                                    i,
                                                                                    ct
                                                                                ),
                                                                            ct
                                                                        ),
                                                                ct
                                                            ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiOpenLinkToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected,
                                i =>
                                    i.Link.IfNotNull(nameof(i.Link))
                                        .IfSuccessAsync(
                                            link => openerLink.OpenLinkAsync(link.ToUri(), ct),
                                            ct
                                        ),
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiAddToFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.AddFavoriteToDoItemAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiRemoveFromFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected.Select(x => x.Id),
                                i => toDoService.RemoveFavoriteToDoItemAsync(i, ct),
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        MultiCompleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccessAsync(
                        selected =>
                            taskProgressService.RunProgressAsync(
                                selected,
                                i =>
                                    i.IsCan switch
                                    {
                                        ToDoItemIsCan.None => Result.AwaitableSuccess,
                                        ToDoItemIsCan.CanComplete
                                            => toDoService.UpdateToDoItemCompleteStatusAsync(
                                                i.Id,
                                                true,
                                                ct
                                            ),
                                        ToDoItemIsCan.CanIncomplete
                                            => toDoService.UpdateToDoItemCompleteStatusAsync(
                                                i.Id,
                                                false,
                                                ct
                                            ),
                                        _
                                            => new Result(new ToDoItemIsCanOutOfRangeError(i.IsCan))
                                                .ToValueTaskResult()
                                                .ConfigureAwait(false),
                                    },
                                ct
                            ),
                        ct
                    )
                    .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct), ct),
            errorHandler,
            taskProgressService
        );

        SwitchPane = SpravyCommand.Create(
            ct =>
                ct.InvokeUiBackgroundAsync(() =>
                {
                    rootViewFactory.CreateMainSplitViewModel().IsPaneOpen = !rootViewFactory
                        .CreateMainSplitViewModel()
                        .IsPaneOpen;

                    return Result.Success;
                }),
            errorHandler,
            taskProgressService
        );

        AddRootToDoItem = SpravyCommand.Create(
            ct =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateAddToDoItemViewModel(),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(vm.ConverterToAddToDoItemOptions, ct)
                            .IfSuccessAsync(
                                options => toDoService.AddToDoItemAsync(options, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Logout = SpravyCommand.Create(
            ct =>
                objectStorage
                    .IsExistsAsync(StorageIds.LoginId, ct)
                    .IfSuccessAsync(
                        value =>
                        {
                            if (value)
                            {
                                return objectStorage.DeleteAsync(StorageIds.LoginId, ct);
                            }

                            return Result.AwaitableSuccess;
                        },
                        ct
                    )
                    .IfSuccessAsync(
                        () => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct),
                        ct
                    )
                    .IfSuccessAsync(
                        () =>
                            ct.InvokeUiBackgroundAsync(() =>
                            {
                                rootViewFactory.CreateMainSplitViewModel().IsPaneOpen = false;

                                return Result.Success;
                            }),
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

        SetToDoItemDescription = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateEditDescriptionViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.UpdateToDoItemDescriptionAsync(
                                        item.Id,
                                        vm.Content.Description,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    toDoService.UpdateToDoItemDescriptionTypeAsync(
                                        item.Id,
                                        vm.Content.DescriptionType,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        AddPasswordItem = SpravyCommand.Create(
            ct =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateAddPasswordItemViewModel(),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    passwordService.AddPasswordItemAsync(
                                        vm.ToAddPasswordOptions(),
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ShowPasswordItemSetting = SpravyCommand.Create<PasswordItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreatePasswordItemSettingsViewModel(item),
                    _ =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemKeyAsync(
                                        item.Id,
                                        item.Key,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemLengthAsync(
                                        item.Id,
                                        item.Length,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemNameAsync(
                                        item.Id,
                                        item.Name,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemRegexAsync(
                                        item.Id,
                                        item.Regex,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemCustomAvailableCharactersAsync(
                                        item.Id,
                                        item.CustomAvailableCharacters,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableNumberAsync(
                                        item.Id,
                                        item.IsAvailableNumber,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableLowerLatinAsync(
                                        item.Id,
                                        item.IsAvailableLowerLatin,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
                                        item.Id,
                                        item.IsAvailableSpecialSymbols,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableUpperLatinAsync(
                                        item.Id,
                                        item.IsAvailableUpperLatin,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        NavigateToActiveToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                toDoService
                    .GetActiveToDoItemAsync(item.Id, ct)
                    .IfSuccessAsync(
                        active =>
                        {
                            if (active.TryGetValue(out var value))
                            {
                                if (value.ParentId.TryGetValue(out var parentId))
                                {
                                    return toDoCache
                                        .GetToDoItem(parentId)
                                        .IfSuccessAsync(
                                            parent =>
                                                navigator.NavigateToAsync(
                                                    viewFactory.CreateToDoItemViewModel(parent),
                                                    ct
                                                ),
                                            ct
                                        );
                                }

                                return navigator.NavigateToAsync(
                                    viewFactory.CreateRootToDoItemsViewModel(),
                                    ct
                                );
                            }

                            return uiApplicationService.RefreshCurrentViewAsync(ct);
                        },
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        ForgotPasswordViewInitialized = SpravyCommand.Create<ForgotPasswordViewModel>(
            (vm, ct) =>
                vm.IdentifierType switch
                {
                    UserIdentifierType.Email
                        => authenticationService.UpdateVerificationCodeByEmailAsync(
                            vm.EmailOrLogin,
                            ct
                        ),
                    UserIdentifierType.Login
                        => authenticationService.UpdateVerificationCodeByLoginAsync(
                            vm.EmailOrLogin,
                            ct
                        ),
                    _
                        => new Result(new UserIdentifierTypeOutOfRangeError(vm.IdentifierType))
                            .ToValueTaskResult()
                            .ConfigureAwait(false),
                },
            errorHandler,
            taskProgressService
        );

        ForgotPassword = SpravyCommand.Create<ForgotPasswordViewModel>(
            (vm, ct) =>
                Result
                    .AwaitableSuccess.IfSuccessAsync(
                        () =>
                            vm.IdentifierType switch
                            {
                                UserIdentifierType.Email
                                    => authenticationService.UpdatePasswordByEmailAsync(
                                        vm.EmailOrLogin,
                                        vm.VerificationCode.ToUpperInvariant(),
                                        vm.NewPassword,
                                        ct
                                    ),
                                UserIdentifierType.Login
                                    => authenticationService.UpdatePasswordByLoginAsync(
                                        vm.EmailOrLogin,
                                        vm.VerificationCode.ToUpperInvariant(),
                                        vm.NewPassword,
                                        ct
                                    ),
                                _
                                    => new Result(
                                        new UserIdentifierTypeOutOfRangeError(vm.IdentifierType)
                                    )
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false),
                            },
                        ct
                    )
                    .IfSuccessAsync(
                        () => navigator.NavigateToAsync(viewFactory.CreateLoginViewModel(), ct),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        CreateUserViewEnter = SpravyCommand.Create<CreateUserView>(
            (view, ct) =>
                this.InvokeUiAsync(
                        () =>
                            view.FindControl<TextBox>(CreateUserView.EmailTextBoxName)
                                .IfNotNull(CreateUserView.EmailTextBoxName)
                                .IfSuccess(emailTextBox =>
                                    view.FindControl<TextBox>(CreateUserView.LoginTextBoxName)
                                        .IfNotNull(CreateUserView.LoginTextBoxName)
                                        .IfSuccess(loginTextBox =>
                                            view.FindControl<TextBox>(
                                                    CreateUserView.PasswordTextBoxName
                                                )
                                                .IfNotNull(CreateUserView.PasswordTextBoxName)
                                                .IfSuccess(passwordTextBox =>
                                                    view.FindControl<TextBox>(
                                                            CreateUserView.RepeatPasswordTextBoxName
                                                        )
                                                        .IfNotNull(
                                                            CreateUserView.RepeatPasswordTextBoxName
                                                        )
                                                        .IfSuccess(repeatPasswordTextBox =>
                                                            new
                                                            {
                                                                EmailTextBox = emailTextBox,
                                                                PasswordTextBox = passwordTextBox,
                                                                LoginTextBox = loginTextBox,
                                                                RepeatPasswordTextBox = repeatPasswordTextBox,
                                                            }.ToResult()
                                                        )
                                                )
                                        )
                                )
                    )
                    .IfSuccessAsync(
                        controls =>
                        {
                            if (controls.EmailTextBox.IsFocused)
                            {
                                return this.InvokeUiAsync(() =>
                                {
                                    controls.LoginTextBox.Focus();

                                    return Result.Success;
                                });
                            }

                            if (controls.LoginTextBox.IsFocused)
                            {
                                return this.InvokeUiAsync(() =>
                                {
                                    controls.PasswordTextBox.Focus();

                                    return Result.Success;
                                });
                            }

                            if (controls.PasswordTextBox.IsFocused)
                            {
                                return this.InvokeUiAsync(() =>
                                {
                                    controls.RepeatPasswordTextBox.Focus();

                                    return Result.Success;
                                });
                            }

                            return this.InvokeUiAsync(
                                    () =>
                                        view.DataContext.CastObject<CreateUserViewModel>(
                                            nameof(view.DataContext)
                                        )
                                )
                                .IfSuccessAsync(
                                    vm =>
                                    {
                                        if (vm.HasErrors)
                                        {
                                            return Result.AwaitableSuccess;
                                        }

                                        return CreateUserAsync(vm, authenticationService, ct);
                                    },
                                    ct
                                );
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
                    () =>
                        objectStorage
                            .IsExistsAsync(StorageIds.LoginId, ct)
                            .IfSuccessAsync(
                                value =>
                                {
                                    if (!value)
                                    {
                                        return this.PostUiBackground(
                                                () => view.FocusTextBoxUi("LoginTextBox"),
                                                ct
                                            )
                                            .ToValueTaskResult()
                                            .ConfigureAwait(false);
                                    }

                                    return objectStorage
                                        .GetObjectAsync<LoginStorageItem>(StorageIds.LoginId, ct)
                                        .IfSuccessAsync(
                                            item =>
                                            {
                                                var jwtHandler = new JwtSecurityTokenHandler();
                                                var token = item.Token;
                                                var jwtToken = jwtHandler.ReadJwtToken(token);
                                                accountNotify.Login = jwtToken.Claims.GetName();

                                                return tokenService
                                                    .LoginAsync(
                                                        item.Token.ThrowIfNullOrWhiteSpace(),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            toDoUiService.UpdateSelectorItemsAsync(
                                                                null,
                                                                ReadOnlyMemory<Guid>.Empty,
                                                                ct
                                                            ),
                                                        ct
                                                    )
                                                    .IfSuccessAsync(
                                                        () =>
                                                            navigator.NavigateToAsync(
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
                    () =>
                        this.InvokeUiBackgroundAsync(() =>
                            {
                                view.ViewModel.IsBusy = false;

                                return Result.Success;
                            })
                            .ToValueTask()
                            .ConfigureAwait(false),
                    ct
                );
            },
            errorHandler,
            taskProgressService
        );

        Login = SpravyCommand.Create<LoginViewModel>(
            (vm, ct) =>
                LoginAsync(
                        vm,
                        authenticationService,
                        tokenService,
                        objectStorage,
                        toDoUiService,
                        accountNotify,
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
            (vm, ct) =>
                vm
                    .View.CastObject<LoginView>(nameof(vm.View))
                    .IfSuccessAsync(
                        view =>
                        {
                            if (this.GetUiValue(() => view.LoginTextBox.IsFocused))
                            {
                                return this.InvokeUiAsync(() =>
                                {
                                    view.PasswordTextBox.FocusTextBoxUi();

                                    return Result.Success;
                                });
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

        PasswordGeneratorViewInitialized = SpravyCommand.Create<PasswordGeneratorViewModel>(
            (vm, ct) => vm.RefreshAsync(ct),
            errorHandler,
            taskProgressService
        );

        CopyToClipboard = SpravyCommand.Create<string>(
            (str, ct) =>
                clipboardService
                    .SetTextAsync(str)
                    .IfSuccessAsync(
                        () =>
                            spravyNotificationManager.ShowAsync(
                                new TextLocalization("Notification.CopyToClipboard"),
                                ct
                            ),
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

        VerificationCodeViewModelInitialized = SpravyCommand.Create<VerificationCodeViewModel>(
            (vm, ct) =>
                vm.IdentifierType switch
                {
                    UserIdentifierType.Email
                        => authenticationService.UpdateVerificationCodeByEmailAsync(
                            vm.EmailOrLogin,
                            ct
                        ),
                    UserIdentifierType.Login
                        => authenticationService.UpdateVerificationCodeByLoginAsync(
                            vm.EmailOrLogin,
                            ct
                        ),
                    _
                        => new Result(new UserIdentifierTypeOutOfRangeError(vm.IdentifierType))
                            .ToValueTaskResult()
                            .ConfigureAwait(false),
                },
            errorHandler,
            taskProgressService
        );

        UpdateEmail = SpravyCommand.Create<IVerificationEmail>(
            (verificationEmail, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Input,
                    viewFactory.CreateTextViewModel(),
                    text =>
                        verificationEmail.IdentifierType switch
                        {
                            UserIdentifierType.Email
                                => authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
                                    verificationEmail.EmailOrLogin,
                                    text.Text,
                                    ct
                                ),
                            UserIdentifierType.Login
                                => authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
                                    verificationEmail.EmailOrLogin,
                                    text.Text,
                                    ct
                                ),
                            _
                                => new Result(
                                    new UserIdentifierTypeOutOfRangeError(
                                        verificationEmail.IdentifierType
                                    )
                                )
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false),
                        },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        VerificationEmail = SpravyCommand.Create<IVerificationEmail>(
            (verificationEmail, ct) =>
                verificationEmail.IdentifierType switch
                {
                    UserIdentifierType.Email
                        => authenticationService
                            .VerifiedEmailByEmailAsync(
                                verificationEmail.EmailOrLogin,
                                verificationEmail.VerificationCode.ToUpperInvariant(),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    navigator.NavigateToAsync(
                                        viewFactory.CreateLoginViewModel(),
                                        ct
                                    ),
                                ct
                            ),
                    UserIdentifierType.Login
                        => authenticationService
                            .VerifiedEmailByLoginAsync(
                                verificationEmail.EmailOrLogin,
                                verificationEmail.VerificationCode.ToUpperInvariant(),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    navigator.NavigateToAsync(
                                        viewFactory.CreateLoginViewModel(),
                                        ct
                                    ),
                                ct
                            ),
                    _
                        => new Result(
                            new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType)
                        )
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
            ct =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateAddTimerViewModel(),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(() => vm.CreateAddTimerParametersAsync(ct), ct)
                            .IfSuccessAsync(
                                parameters => scheduleService.AddTimerAsync(parameters, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        DeleteTimer = SpravyCommand.Create<TimerItemNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateDeleteTimerViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () => scheduleService.RemoveTimerAsync(vm.Item.Id, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CreateTimer = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateToDoItemCreateTimerViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(() => vm.CreateAddTimerParametersAsync(ct), ct)
                            .IfSuccessAsync(
                                parameters => scheduleService.AddTimerAsync(parameters, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
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
                var key = TypeCache<SettingViewModel>.Type.Name;

                return objectStorage
                    .IsExistsAsync(key, ct)
                    .IfSuccessAsync(
                        isExists =>
                        {
                            if (isExists)
                            {
                                return objectStorage
                                    .GetObjectAsync<Setting.Setting>(key, ct)
                                    .IfSuccessAsync(
                                        setting =>
                                            setting.PostUiBackground(
                                                () =>
                                                {
                                                    application.RequestedThemeVariant =
                                                        setting.Theme.ToThemeVariant();

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
    }

    public SpravyCommand MultiCompleteToDoItem { get; }
    public SpravyCommand MultiAddToFavoriteToDoItem { get; }
    public SpravyCommand MultiRemoveFromFavoriteToDoItem { get; }
    public SpravyCommand MultiOpenLinkToDoItem { get; }
    public SpravyCommand MultiAddChildToDoItem { get; }
    public SpravyCommand MultiDeleteToDoItem { get; }
    public SpravyCommand MultiShowSettingToDoItem { get; }
    public SpravyCommand MultiOpenLeafToDoItem { get; }
    public SpravyCommand MultiChangeParentToDoItem { get; }
    public SpravyCommand MultiMakeAsRootToDoItem { get; }
    public SpravyCommand MultiCopyToClipboardToDoItem { get; }
    public SpravyCommand MultiRandomizeChildrenOrderToDoItem { get; }
    public SpravyCommand MultiChangeOrderToDoItem { get; }
    public SpravyCommand MultiResetToDoItem { get; }
    public SpravyCommand MultiCloneToDoItem { get; }
    public SpravyCommand MultiCreateReferenceToDoItem { get; }

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
    public SpravyCommand MakeAsRoot { get; }
    public SpravyCommand ToDoItemCopyToClipboard { get; }
    public SpravyCommand RandomizeChildrenOrder { get; }
    public SpravyCommand ChangeOrder { get; }
    public SpravyCommand Reset { get; }
    public SpravyCommand Clone { get; }
    public SpravyCommand CreateReference { get; }
    public SpravyCommand NavigateToToDoItem { get; }
    public SpravyCommand NavigateToActiveToDoItem { get; }

    public SpravyCommand MultiComplete { get; }
    public SpravyCommand MultiAddToFavorite { get; }
    public SpravyCommand MultiRemoveFromFavorite { get; }
    public SpravyCommand MultiAddChild { get; }
    public SpravyCommand MultiDelete { get; }
    public SpravyCommand MultiShowSetting { get; }
    public SpravyCommand MultiOpenLeaf { get; }
    public SpravyCommand MultiChangeParent { get; }
    public SpravyCommand MultiMakeAsRoot { get; }
    public SpravyCommand MultiCopyToClipboard { get; }
    public SpravyCommand MultiRandomizeChildrenOrder { get; }
    public SpravyCommand MultiChangeOrder { get; }
    public SpravyCommand MultiReset { get; }
    public SpravyCommand MultiClone { get; }
    public SpravyCommand MultiCreateReference { get; }
    public SpravyCommand MultiOpenLink { get; }

    public SpravyCommand GeneratePassword { get; }
    public SpravyCommand DeletePasswordItem { get; }
    public SpravyCommand AddPasswordItem { get; }
    public SpravyCommand ShowPasswordItemSetting { get; }

    public SpravyCommand SwitchPane { get; }
    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand NavigateToCurrentToDoItem { get; }
    public SpravyCommand AddRootToDoItem { get; }
    public SpravyCommand Logout { get; }
    public SpravyCommand RefreshCurrentView { get; }
    public SpravyCommand SetToDoItemDescription { get; }

    public SpravyCommand ForgotPasswordViewInitialized { get; }
    public SpravyCommand LoginViewInitialized { get; }
    public SpravyCommand PasswordGeneratorViewInitialized { get; }

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

    public SpravyCommand VerificationCodeViewModelInitialized { get; }
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

        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
            .IfSuccessTryFinallyAsync(
                () =>
                    authenticationService
                        .CreateUserAsync(viewModel.ToCreateUserOptions(), ct)
                        .IfSuccessAsync(
                            () =>
                                navigator.NavigateToAsync(
                                    viewFactory.CreateVerificationCodeViewModel(
                                        viewModel.Email,
                                        UserIdentifierType.Email
                                    ),
                                    ct
                                ),
                            ct
                        ),
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                        {
                            viewModel.IsBusy = false;

                            return Result.Success;
                        })
                        .ToValueTask()
                        .ConfigureAwait(false),
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
        CancellationToken ct
    )
    {
        if (viewModel.HasErrors)
        {
            return Result.AwaitableSuccess;
        }

        return this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.IsBusy = true;

                return Result.Success;
            })
            .IfSuccessTryFinallyAsync(
                () =>
                    authenticationService
                        .IsVerifiedByLoginAsync(viewModel.Login, ct)
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

                                return tokenService
                                    .LoginAsync(viewModel.ToUser(), ct)
                                    .IfSuccessAsync(
                                        () =>
                                            this.InvokeUiBackgroundAsync(() =>
                                                {
                                                    accountNotify.Login = viewModel.Login;

                                                    return Result.Success;
                                                })
                                                .IfSuccessAsync(
                                                    () =>
                                                        RememberMeAsync(
                                                            viewModel,
                                                            tokenService,
                                                            objectStorage,
                                                            ct
                                                        ),
                                                    ct
                                                )
                                                .IfSuccessAsync(
                                                    () =>
                                                        toDoUiService.UpdateSelectorItemsAsync(
                                                            null,
                                                            ReadOnlyMemory<Guid>.Empty,
                                                            ct
                                                        ),
                                                    ct
                                                )
                                                .IfSuccessAsync(
                                                    () =>
                                                        navigator.NavigateToAsync(
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
                () =>
                    this.InvokeUiBackgroundAsync(() =>
                        {
                            viewModel.IsBusy = false;

                            return Result.Success;
                        })
                        .ToValueTask()
                        .ConfigureAwait(false),
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

        return tokenService
            .GetTokenAsync(ct)
            .IfSuccessAsync(
                token =>
                {
                    var item = new LoginStorageItem { Token = token };

                    return objectStorage.SaveObjectAsync(StorageIds.LoginId, item, ct);
                },
                ct
            );
    }
}
