using Spravy.Core.Mappers;
using Spravy.Ui.Mappers;

namespace Spravy.Ui.Services;

public class SpravyCommandService
{
    private readonly INavigator navigator;
    private readonly IErrorHandler errorHandler;
    private readonly ITaskProgressService taskProgressService;
    private readonly Dictionary<Type, SpravyCommand> createNavigateToCache;

    public SpravyCommandService(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IToDoCache toDoCache,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IAuthenticationService authenticationService,
        IPasswordService passwordService,
        MainSplitViewModel mainSplitViewModel,
        IObjectStorage objectStorage,
        SukiTheme sukiTheme,
        AccountNotify accountNotify,
        ITokenService tokenService,
        ISpravyNotificationManager spravyNotificationManager,
        INavigator navigator,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        createNavigateToCache = new();
        this.navigator = navigator;
        this.errorHandler = errorHandler;
        this.taskProgressService = taskProgressService;

        NavigateToToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = item.CurrentId, ct),
            errorHandler,
            taskProgressService
        );

        MultiCreateReference = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (property, ct) =>
                property
                    .GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selectedIds =>
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService
                                                    .RunProgressAsync(
                                                        selectedIds,
                                                        i =>
                                                            toDoService
                                                                .CloneToDoItemAsync(
                                                                    i,
                                                                    vm.Id.ToOption(),
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
                                vm => vm.IgnoreIds = selectedIds,
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
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService
                                                    .RunProgressAsync(
                                                        selectedIds,
                                                        i =>
                                                            toDoService.CloneToDoItemAsync(
                                                                i,
                                                                vm.Id.ToOption(),
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
                                vm => vm.IgnoreIds = selectedIds,
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiReset = SpravyCommand.Create<IToDoSubItemsViewModelProperty>(
            (view, ct) =>
                view.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                ActionHelper<ResetToDoItemViewModel>.Empty,
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                                viewModel =>
                                    viewModel
                                        .SelectedItem.IfNotNull(nameof(viewModel.SelectedItem))
                                        .IfSuccessAsync(
                                            selectedItem =>
                                                dialogViewer
                                                    .CloseContentDialogAsync(ct)
                                                    .IfSuccessAsync(
                                                        () =>
                                                            taskProgressService.RunProgressAsync(
                                                                selected,
                                                                i =>
                                                                    toDoService.UpdateToDoItemOrderIndexAsync(
                                                                        new(
                                                                            i,
                                                                            selectedItem.Id,
                                                                            viewModel.IsAfter
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                viewModel =>
                                {
                                    viewModel.ChangeToDoItemOrderIndexIds = view
                                        .ToDoSubItemsViewModel.List.ToDoItems.GroupByNone.Items.Items.Where(
                                            x => !x.IsSelected
                                        )
                                        .Select(x => x.Id)
                                        .ToArray();
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                                _ =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.RandomizeChildrenOrderIndexAsync(
                                                            i,
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                viewModel =>
                                {
                                    viewModel.RandomizeChildrenOrderIds = selected;
                                },
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
                            dialogViewer.ShowConfirmContentDialogAsync(
                                vm =>
                                {
                                    var statuses = vm
                                        .Statuses.Where(x => x.IsChecked)
                                        .Select(x => x.Item)
                                        .ToArray();

                                    return dialogViewer
                                        .CloseContentDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.ToDoItemToStringAsync(
                                                            new(statuses, i),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            items =>
                                                clipboardService.SetTextAsync(
                                                    items.Join(Environment.NewLine).ToString()
                                                ),
                                            ct
                                        );
                                },
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty,
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.UpdateToDoItemParentAsync(
                                                            i,
                                                            vm.Id,
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
                                viewModel =>
                                {
                                    viewModel.IgnoreIds = selected;
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            navigator.NavigateToAsync<LeafToDoItemsViewModel>(
                                vm =>
                                {
                                    vm.LeafIds = selected;
                                },
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
                            dialogViewer.ShowConfirmContentDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                ActionHelper<MultiToDoItemSettingViewModel>.Empty,
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
                            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                                _ =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                vm =>
                                {
                                    vm.DeleteItems.Update(selected);
                                },
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
                            dialogViewer.ShowConfirmContentDialogAsync(
                                viewModel =>
                                    taskProgressService.RunProgressAsync(
                                        selected,
                                        item =>
                                            viewModel
                                                .ConverterToAddToDoItemOptions(item.Id)
                                                .IfSuccessAsync(
                                                    options =>
                                                        dialogViewer
                                                            .CloseContentDialogAsync(ct)
                                                            .IfSuccessAsync(
                                                                () =>
                                                                    toDoService.AddToDoItemAsync(
                                                                        options,
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                ActionHelper<AddToDoItemViewModel>.Empty,
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
                                    return navigator.NavigateToAsync<ToDoItemViewModel>(
                                        viewModel => viewModel.Id = parentId,
                                        ct
                                    );
                                }
                            }

                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(ct);
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
                            verificationEmail.Identifier,
                            ct
                        ),
                    UserIdentifierType.Login
                        => authenticationService.UpdateVerificationCodeByLoginAsync(
                            verificationEmail.Identifier,
                            ct
                        ),
                    _
                        => new Result(
                            new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType)
                        )
                            .ToValueTaskResult()
                            .ConfigureAwait(false)
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
                dialogViewer.ShowConfirmContentDialogAsync<DeletePasswordItemViewModel>(
                    _ =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () => passwordService.DeletePasswordItemAsync(passwordItem.Id, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    view => view.PasswordItemId = passwordItem.Id,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Complete = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
            {
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
                                                .GetCurrentView<IToDoItemUpdater>()
                                                .IfSuccess(u => u.UpdateInListToDoItemUi(item));
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
                                                .GetCurrentView<IToDoItemUpdater>()
                                                .IfSuccess(u => u.UpdateInListToDoItemUi(item));
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
                                        .ConfigureAwait(false)
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
                dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                    viewModel =>
                        viewModel
                            .ConverterToAddToDoItemOptions()
                            .IfSuccessAsync(
                                options =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () => toDoService.AddToDoItemAsync(options, ct),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                            ct
                                        ),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    vm => vm.ParentId = item.CurrentId,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Delete = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                    _ =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
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
                                        return navigator.NavigateToAsync<RootToDoItemsViewModel>(
                                            ct
                                        );
                                    }

                                    return navigator.NavigateToAsync<ToDoItemViewModel>(
                                        viewModel => viewModel.Id = item.Parent.Id,
                                        ct
                                    );
                                },
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    view => view.Item = item,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ShowSetting = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
                    vm =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
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
                                () => vm.Settings.ThrowIfNull().ApplySettingsAsync(ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    vm => vm.ToDoItemId = item.Id,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        OpenLeaf = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Item = item, ct),
            errorHandler,
            taskProgressService
        );

        ChangeParent = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                    i =>
                        dialogViewer
                            .CloseInputDialogAsync(ct)
                            .IfSuccessAsync(
                                () => toDoService.UpdateToDoItemParentAsync(item.Id, i.Id, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    viewModel =>
                    {
                        viewModel.IgnoreIds = new([item.Id,]);
                        viewModel.DefaultSelectedItemId = (item.Parent?.Id).GetValueOrDefault();
                    },
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
                                ActionHelper<RootToDoItemsViewModel>.Empty,
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        CopyToClipboard = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync(
                    view =>
                    {
                        var statuses = view
                            .Statuses.Where(x => x.IsChecked)
                            .Select(x => x.Item)
                            .ToArray();

                        var options = new ToDoItemToStringOptions(statuses, item.CurrentId);

                        return dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService
                                        .ToDoItemToStringAsync(options, ct)
                                        .IfSuccessAsync(clipboardService.SetTextAsync, ct),
                                ct
                            );
                    },
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    ActionHelper<ToDoItemToStringSettingsViewModel>.Empty,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        RandomizeChildrenOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                    _ =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
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
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    viewModel => viewModel.Item = item,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ChangeOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                    viewModel =>
                    {
                        var targetId = viewModel.SelectedItem.ThrowIfNull().Id;

                        var options = new UpdateOrderIndexToDoItemOptions(
                            viewModel.Id,
                            targetId,
                            viewModel.IsAfter
                        );

                        return dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () => toDoService.UpdateToDoItemOrderIndexAsync(options, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            );
                    },
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    viewModel => viewModel.Id = item.Id,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        Reset = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                    vm =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.ResetToDoItemAsync(vm.ToResetToDoItemOptions(), ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    vm => vm.Id = item.CurrentId,
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
                dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                    itemNotify =>
                        dialogViewer
                            .CloseInputDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.CloneToDoItemAsync(
                                        item.Id,
                                        itemNotify.Id.ToOption(),
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    view => view.DefaultSelectedItemId = item.Id,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        CreateReference = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                    itemNotify =>
                        dialogViewer
                            .CloseInputDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService
                                        .CloneToDoItemAsync(item.Id, itemNotify.Id.ToOption(), ct)
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
                    view => view.DefaultSelectedItemId = item.Id,
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
                            dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                                viewModel =>
                                    taskProgressService.RunProgressAsync(
                                        selected,
                                        i =>
                                            viewModel
                                                .ConverterToAddToDoItemOptions(i.Id)
                                                .IfSuccessAsync(
                                                    options =>
                                                        dialogViewer
                                                            .CloseContentDialogAsync(ct)
                                                            .IfSuccessAsync(
                                                                () =>
                                                                    toDoService.AddToDoItemAsync(
                                                                        options,
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                vm => vm.ParentId = item.CurrentId,
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
                            dialogViewer.ShowConfirmContentDialogAsync<MultiToDoItemSettingViewModel>(
                                vm =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                vm => vm.ToDoItemId = item.Id,
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
                            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                                _ =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                vm =>
                                {
                                    vm.Item = item;
                                    vm.DeleteItems.Update(selected);
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selectedIds =>
                            navigator.NavigateToAsync<LeafToDoItemsViewModel>(
                                vm =>
                                {
                                    vm.Item = item;
                                    vm.LeafIds = selectedIds;
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                vm =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.UpdateToDoItemParentAsync(
                                                            i,
                                                            vm.Id,
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
                                viewModel =>
                                {
                                    viewModel.IgnoreIds = selected;
                                    viewModel.DefaultSelectedItemId = item.Id;
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync(
                                view =>
                                {
                                    var statuses = view
                                        .Statuses.Where(x => x.IsChecked)
                                        .Select(x => x.Item)
                                        .ToArray();

                                    return dialogViewer
                                        .CloseContentDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.ToDoItemToStringAsync(
                                                            new(statuses, i),
                                                            ct
                                                        ),
                                                    ct
                                                ),
                                            ct
                                        )
                                        .IfSuccessAsync(
                                            items =>
                                                clipboardService.SetTextAsync(
                                                    items.Join(Environment.NewLine).ToString()
                                                ),
                                            ct
                                        );
                                },
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty,
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                                _ =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.RandomizeChildrenOrderIndexAsync(
                                                            i,
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                viewModel =>
                                {
                                    viewModel.Item = item;
                                    viewModel.RandomizeChildrenOrderIds = selected;
                                },
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
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                                viewModel =>
                                    viewModel
                                        .SelectedItem.IfNotNull(nameof(viewModel.SelectedItem))
                                        .IfSuccessAsync(
                                            selectedItem =>
                                                dialogViewer
                                                    .CloseContentDialogAsync(ct)
                                                    .IfSuccessAsync(
                                                        () =>
                                                            taskProgressService.RunProgressAsync(
                                                                selected,
                                                                i =>
                                                                    toDoService.UpdateToDoItemOrderIndexAsync(
                                                                        new(
                                                                            i,
                                                                            selectedItem.Id,
                                                                            viewModel.IsAfter
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                viewModel =>
                                {
                                    viewModel.Id = item.Id;

                                    viewModel.ChangeToDoItemOrderIndexIds = item
                                        .Children.Where(x => !x.IsSelected)
                                        .Select(x => x.Id)
                                        .ToArray();
                                },
                                ct
                            ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        MultiResetToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                item.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                                vm =>
                                    dialogViewer
                                        .CloseContentDialogAsync(ct)
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
                                _ => dialogViewer.CloseContentDialogAsync(ct),
                                vm => vm.Id = item.CurrentId,
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
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                itemNotify =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService.CloneToDoItemAsync(
                                                            i,
                                                            itemNotify.Id.ToOption(),
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
                                view => view.DefaultSelectedItemId = item.Id,
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
                            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                                itemNotify =>
                                    dialogViewer
                                        .CloseInputDialogAsync(ct)
                                        .IfSuccessAsync(
                                            () =>
                                                taskProgressService.RunProgressAsync(
                                                    selected,
                                                    i =>
                                                        toDoService
                                                            .CloneToDoItemAsync(
                                                                i,
                                                                itemNotify.Id.ToOption(),
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
                                view => view.DefaultSelectedItemId = item.Id,
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
                                                .ConfigureAwait(false)
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
                    mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen;

                    return Result.Success;
                }),
            errorHandler,
            taskProgressService
        );

        AddRootToDoItem = SpravyCommand.Create(
            ct =>
                dialogViewer.ShowConfirmContentDialogAsync(
                    view =>
                    {
                        var options = view.ToAddRootToDoItemOptions();

                        return dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(() => toDoService.AddRootToDoItemAsync(options, ct), ct)
                            .IfSuccessAsync(
                                _ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            );
                    },
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    ActionHelper<AddRootToDoItemViewModel>.Empty,
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
                        () => navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, ct),
                        ct
                    )
                    .IfSuccessAsync(
                        () =>
                            ct.InvokeUiBackgroundAsync(() =>
                            {
                                mainSplitViewModel.IsPaneOpen = false;

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
                dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
                    viewModel =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.UpdateToDoItemDescriptionAsync(
                                        item.Id,
                                        viewModel.Content.Description,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    toDoService.UpdateToDoItemDescriptionTypeAsync(
                                        item.Id,
                                        viewModel.Content.Type,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    viewModel =>
                    {
                        viewModel.Content.Description = item.Description;
                        viewModel.Content.Type = item.DescriptionType;
                        viewModel.ToDoItemName = item.Name;
                    },
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        AddPasswordItem = SpravyCommand.Create(
            ct =>
                dialogViewer.ShowConfirmContentDialogAsync(
                    vm =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
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
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    ActionHelper<AddPasswordItemViewModel>.Empty,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        ShowPasswordItemSetting = SpravyCommand.Create<PasswordItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<PasswordItemSettingsViewModel>(
                    vm =>
                        dialogViewer
                            .CloseContentDialogAsync(ct)
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemKeyAsync(item.Id, vm.Key, ct),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemLengthAsync(
                                        item.Id,
                                        vm.Length,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemNameAsync(
                                        item.Id,
                                        vm.Name,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemRegexAsync(
                                        item.Id,
                                        vm.Regex,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemCustomAvailableCharactersAsync(
                                        item.Id,
                                        vm.CustomAvailableCharacters,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableNumberAsync(
                                        item.Id,
                                        vm.IsAvailableNumber,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableLowerLatinAsync(
                                        item.Id,
                                        vm.IsAvailableLowerLatin,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
                                        item.Id,
                                        vm.IsAvailableSpecialSymbols,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () =>
                                    passwordService.UpdatePasswordItemIsAvailableUpperLatinAsync(
                                        item.Id,
                                        vm.IsAvailableUpperLatin,
                                        ct
                                    ),
                                ct
                            )
                            .IfSuccessAsync(
                                () => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct
                            ),
                    _ => dialogViewer.CloseContentDialogAsync(ct),
                    vm => vm.Id = item.Id,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        SettingViewInitialized = SpravyCommand.Create<SettingViewModel>(
            (viewModel, _) =>
                this.InvokeUiBackgroundAsync(() =>
                {
                    viewModel.AvailableColors.Clear();
                    viewModel.AvailableColors.AddRange(
                        sukiTheme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x))
                    );

                    foreach (var availableColor in viewModel.AvailableColors)
                    {
                        if (availableColor.Value == sukiTheme.ActiveColorTheme)
                        {
                            availableColor.IsSelect = true;
                        }
                    }

                    viewModel.IsLightTheme = sukiTheme.ActiveBaseTheme == ThemeVariant.Light;

                    return Result.Success;
                }),
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
                                    return navigator.NavigateToAsync<ToDoItemViewModel>(
                                        vm => vm.Id = parentId,
                                        ct
                                    );
                                }

                                return navigator.NavigateToAsync<RootToDoItemsViewModel>(ct);
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
                            vm.Identifier,
                            ct
                        ),
                    UserIdentifierType.Login
                        => authenticationService.UpdateVerificationCodeByLoginAsync(
                            vm.Identifier,
                            ct
                        ),
                    _
                        => new Result(new UserIdentifierTypeOutOfRangeError(vm.IdentifierType))
                            .ToValueTaskResult()
                            .ConfigureAwait(false)
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
                                        vm.Identifier,
                                        vm.VerificationCode.ToUpperInvariant(),
                                        vm.NewPassword,
                                        ct
                                    ),
                                UserIdentifierType.Login
                                    => authenticationService.UpdatePasswordByLoginAsync(
                                        vm.Identifier,
                                        vm.VerificationCode.ToUpperInvariant(),
                                        vm.NewPassword,
                                        ct
                                    ),
                                _
                                    => new Result(
                                        new UserIdentifierTypeOutOfRangeError(vm.IdentifierType)
                                    )
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                            },
                        ct
                    )
                    .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct),
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
                                    () => view.ViewModel.IfNotNull(nameof(view.ViewModel))
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

        LoginViewInitialized = SpravyCommand.Create<LoginViewModel>(
            (viewModel, ct) =>
                this.InvokeUiBackgroundAsync(() =>
                    {
                        viewModel.IsBusy = true;

                        return Result.Success;
                    })
                    .IfSuccessTryFinallyAsync(
                        () =>
                        {
                            return objectStorage
                                .GetObjectOrDefaultAsync<LoginViewModelSetting>(
                                    viewModel.ViewId,
                                    ct
                                )
                                .IfSuccessAsync(
                                    setting =>
                                    {
                                        return viewModel
                                            .SetStateAsync(setting, ct)
                                            .IfSuccessAsync(
                                                () =>
                                                    objectStorage.IsExistsAsync(
                                                        StorageIds.LoginId,
                                                        ct
                                                    ),
                                                ct
                                            )
                                            .IfSuccessAsync(
                                                value =>
                                                {
                                                    if (!value)
                                                    {
                                                        return Result.AwaitableSuccess;
                                                    }

                                                    return objectStorage
                                                        .GetObjectAsync<LoginStorageItem>(
                                                            StorageIds.LoginId,
                                                            ct
                                                        )
                                                        .IfSuccessAsync(
                                                            item =>
                                                            {
                                                                var jwtHandler =
                                                                    new JwtSecurityTokenHandler();
                                                                var jwtToken =
                                                                    jwtHandler.ReadJwtToken(
                                                                        item.Token
                                                                    );
                                                                var l = jwtToken
                                                                    .Claims.Single(x =>
                                                                        x.Type == ClaimTypes.Name
                                                                    )
                                                                    .Value;
                                                                accountNotify.Login = l;

                                                                return tokenService
                                                                    .LoginAsync(
                                                                        item.Token.ThrowIfNullOrWhiteSpace(),
                                                                        ct
                                                                    )
                                                                    .IfSuccessAsync(
                                                                        () =>
                                                                            toDoService.GetToDoSelectorItemsAsync(
                                                                                ReadOnlyMemory<Guid>.Empty,
                                                                                ct
                                                                            ),
                                                                        ct
                                                                    )
                                                                    .IfSuccessAsync(
                                                                        items =>
                                                                            this.InvokeUiBackgroundAsync(
                                                                                () =>
                                                                                {
                                                                                    toDoCache.UpdateUi(
                                                                                        items
                                                                                    );

                                                                                    return Result.Success;
                                                                                }
                                                                            ),
                                                                        ct
                                                                    )
                                                                    .IfSuccessAsync(
                                                                        () =>
                                                                            navigator.NavigateToAsync(
                                                                                ActionHelper<RootToDoItemsViewModel>.Empty,
                                                                                ct
                                                                            ),
                                                                        ct
                                                                    );
                                                            },
                                                            ct
                                                        );
                                                },
                                                ct
                                            );
                                    },
                                    ct
                                );
                        },
                        () =>
                            this.InvokeUiBackgroundAsync(() =>
                                {
                                    viewModel.IsBusy = false;

                                    return Result.Success;
                                })
                                .ToValueTask()
                                .ConfigureAwait(false),
                        ct
                    ),
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
                    toDoService,
                    toDoCache,
                    accountNotify,
                    ct
                ),
            errorHandler,
            taskProgressService
        );

        LoginViewEnter = SpravyCommand.Create<LoginView>(
            (view, ct) =>
                this.InvokeUiAsync(
                        () =>
                            view.FindControl<TextBox>(LoginView.LoginTextBoxName)
                                .IfNotNull(nameof(LoginView.LoginTextBoxName))
                    )
                    .IfSuccessAsync(
                        loginTextBox =>
                            this.InvokeUiAsync(
                                    () =>
                                        view.FindControl<TextBox>(LoginView.PasswordTextBoxName)
                                            .IfNotNull(LoginView.PasswordTextBoxName)
                                )
                                .IfSuccessAsync(
                                    passwordTextBox =>
                                    {
                                        if (loginTextBox.IsFocused)
                                        {
                                            return this.InvokeUiAsync(() =>
                                            {
                                                passwordTextBox.Focus();

                                                return Result.Success;
                                            });
                                        }

                                        if (passwordTextBox.IsFocused)
                                        {
                                            return this.InvokeUiAsync(
                                                    () =>
                                                        view.ViewModel.IfNotNull(
                                                            nameof(view.ViewModel)
                                                        )
                                                )
                                                .IfSuccessAsync(
                                                    viewModel =>
                                                    {
                                                        if (viewModel.HasErrors)
                                                        {
                                                            return Result.AwaitableSuccess;
                                                        }

                                                        return LoginAsync(
                                                            viewModel,
                                                            authenticationService,
                                                            tokenService,
                                                            objectStorage,
                                                            toDoService,
                                                            toDoCache,
                                                            accountNotify,
                                                            ct
                                                        );
                                                    },
                                                    ct
                                                );
                                        }

                                        return Result.AwaitableSuccess;
                                    },
                                    ct
                                ),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        DeletePasswordItemViewInitialized = SpravyCommand.Create<DeletePasswordItemViewModel>(
            (vm, ct) =>
                passwordService
                    .GetPasswordItemAsync(vm.PasswordItemId, ct)
                    .IfSuccessAsync(
                        value =>
                            this.InvokeUiBackgroundAsync(() =>
                            {
                                vm.PasswordItemName = value.Name;

                                return Result.Success;
                            }),
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

        PasswordItemSettingsViewInitialized = SpravyCommand.Create<PasswordItemSettingsViewModel>(
            (vm, ct) =>
                passwordService
                    .GetPasswordItemAsync(vm.Id, ct)
                    .IfSuccessAsync(
                        value =>
                            this.InvokeUiBackgroundAsync(() =>
                            {
                                vm.Name = value.Name;
                                vm.Regex = value.Regex;
                                vm.Key = value.Key;
                                vm.Length = value.Length;
                                vm.IsAvailableUpperLatin = value.IsAvailableUpperLatin;
                                vm.IsAvailableLowerLatin = value.IsAvailableLowerLatin;
                                vm.IsAvailableNumber = value.IsAvailableNumber;
                                vm.IsAvailableSpecialSymbols = value.IsAvailableSpecialSymbols;
                                vm.CustomAvailableCharacters = value.CustomAvailableCharacters;

                                return Result.Success;
                            }),
                        ct
                    ),
            errorHandler,
            taskProgressService
        );

        AddRootToDoItemViewInitialized = SpravyCommand.Create<AddRootToDoItemViewModel>(
            (vm, ct) =>
                objectStorage
                    .GetObjectOrDefaultAsync<AddRootToDoItemViewModelSetting>(vm.ViewId, ct)
                    .IfSuccessAsync(obj => vm.SetStateAsync(obj, ct), ct),
            errorHandler,
            taskProgressService
        );

        AddToDoItemViewInitialized = SpravyCommand.Create<AddToDoItemViewModel>(
            (vm, ct) =>
                objectStorage
                    .GetObjectOrDefaultAsync<AddToDoItemViewModelSetting>(vm.ViewId, ct)
                    .IfSuccessAsync(obj => vm.SetStateAsync(obj, ct), ct)
                    .IfSuccessAsync(
                        () =>
                            toDoService
                                .GetParentsAsync(vm.ParentId, ct)
                                .IfSuccessAsync(
                                    parents =>
                                    {
                                        var path = MaterialIconKind
                                            .Home.As<object>()
                                            .ToEnumerable()
                                            .Concat(parents.ToArray().Select(x => x.Name))
                                            .Select(x => x.ThrowIfNull())
                                            .ToArray();

                                        return this.InvokeUiBackgroundAsync(() =>
                                        {
                                            vm.Path = path;

                                            return Result.Success;
                                        });
                                    },
                                    ct
                                ),
                        ct
                    ),
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

    public SpravyCommand Complete { get; }
    public SpravyCommand AddToFavorite { get; }
    public SpravyCommand RemoveFromFavorite { get; }
    public SpravyCommand OpenLink { get; }
    public SpravyCommand AddChild { get; }
    public SpravyCommand Delete { get; }
    public SpravyCommand ShowSetting { get; }
    public SpravyCommand OpenLeaf { get; }
    public SpravyCommand ChangeParent { get; }
    public SpravyCommand MakeAsRoot { get; }
    public SpravyCommand CopyToClipboard { get; }
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

    public SpravyCommand SettingViewInitialized { get; }
    public SpravyCommand ForgotPasswordViewInitialized { get; }
    public SpravyCommand LoginViewInitialized { get; }
    public SpravyCommand DeletePasswordItemViewInitialized { get; }
    public SpravyCommand PasswordGeneratorViewInitialized { get; }
    public SpravyCommand PasswordItemSettingsViewInitialized { get; }
    public SpravyCommand AddRootToDoItemViewInitialized { get; }
    public SpravyCommand AddToDoItemViewInitialized { get; }

    public SpravyCommand ForgotPassword { get; }

    public SpravyCommand CreateUserViewEnter { get; }
    public SpravyCommand CreateUser { get; }

    public SpravyCommand Login { get; }
    public SpravyCommand LoginViewEnter { get; }

    public SpravyCommand GetNavigateTo<TViewModel>()
        where TViewModel : INavigatable
    {
        if (createNavigateToCache.TryGetValue(typeof(TViewModel), out var command))
        {
            return command;
        }

        var result = SpravyCommand.Create(
            navigator.NavigateToAsync<TViewModel>,
            errorHandler,
            taskProgressService
        );
        createNavigateToCache.Add(typeof(TViewModel), result);

        return result;
    }

    private ConfiguredValueTaskAwaitable<Result> CreateUserAsync(
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
                                navigator.NavigateToAsync<VerificationCodeViewModel>(
                                    vm =>
                                    {
                                        vm.Identifier = viewModel.Email;
                                        vm.IdentifierType = UserIdentifierType.Email;
                                    },
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

    private ConfiguredValueTaskAwaitable<Result> LoginAsync(
        LoginViewModel viewModel,
        IAuthenticationService authenticationService,
        ITokenService tokenService,
        IObjectStorage objectStorage,
        IToDoService toDoService,
        IToDoCache toDoCache,
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
                                    return navigator.NavigateToAsync<VerificationCodeViewModel>(
                                        vm =>
                                        {
                                            vm.Identifier = viewModel.Login;
                                            vm.IdentifierType = UserIdentifierType.Login;
                                        },
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
                                                        toDoService.GetToDoSelectorItemsAsync(
                                                            ReadOnlyMemory<Guid>.Empty,
                                                            ct
                                                        ),
                                                    ct
                                                )
                                                .IfSuccessAsync(
                                                    items =>
                                                        this.InvokeUiBackgroundAsync(() =>
                                                        {
                                                            toDoCache.UpdateUi(items);

                                                            return Result.Success;
                                                        }),
                                                    ct
                                                )
                                                .IfSuccessAsync(
                                                    () =>
                                                        navigator.NavigateToAsync(
                                                            ActionHelper<RootToDoItemsViewModel>.Empty,
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

    private ConfiguredValueTaskAwaitable<Result> RememberMeAsync(
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
                    var item = new LoginStorageItem { Token = token, };

                    return objectStorage.SaveObjectAsync(StorageIds.LoginId, item, ct);
                },
                ct
            );
    }
}
