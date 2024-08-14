using Spravy.Core.Mappers;
using Spravy.Ui.Features.ToDo.Views;
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
        AccountNotify accountNotify,
        ITokenService tokenService,
        ISpravyNotificationManager spravyNotificationManager,
        INavigator navigator,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService,
        IViewFactory viewFactory,
        IToDoUiService toDoUiService
    )
    {
        createNavigateToCache = new();
        this.navigator = navigator;
        this.errorHandler = errorHandler;
        this.taskProgressService = taskProgressService;

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
                view.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
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
                            navigator.NavigateToAsync<LeafToDoItemsViewModel>(
                                vm => vm.Items.UpdateUi(selected),
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
                                viewFactory.CreateMultiToDoItemSettingViewModel(view.Item),
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
                                        return navigator.NavigateToAsync<RootToDoItemsViewModel>(
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
                navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Item = item, ct),
            errorHandler,
            taskProgressService
        );

        ChangeParent = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Input,
                    viewFactory.CreateToDoItemSelectorViewModel(),
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
                        () => navigator.NavigateToAsync<RootToDoItemsViewModel>(ct),
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
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateResetToDoItemViewModel(item),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.ResetToDoItemAsync(vm.ToResetToDoItemOptions(), ct),
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
                                viewFactory.CreateMultiToDoItemSettingViewModel(item),
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
                        selectedIds =>
                            navigator.NavigateToAsync<LeafToDoItemsViewModel>(
                                vm =>
                                {
                                    vm.Item = item;
                                    vm.Items.UpdateUi(selectedIds);
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
                    .IfSuccessAsync(
                        selected =>
                            dialogViewer.ShowConfirmDialogAsync(
                                viewFactory,
                                DialogViewLayer.Content,
                                viewFactory.CreateToDoItemSelectorViewModel(item, selected),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
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
                item.GetSelectedItems()
                    .IfSuccess(selected => selected.Select(x => x.Id).ToResult())
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
                                DialogViewLayer.Content,
                                viewFactory.CreateToDoItemSelectorViewModel(item),
                                vm =>
                                    dialogViewer
                                        .CloseDialogAsync(DialogViewLayer.Content, ct)
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
                dialogViewer.ShowConfirmDialogAsync(
                    viewFactory,
                    DialogViewLayer.Content,
                    viewFactory.CreateAddRootToDoItemViewModel(),
                    vm =>
                        dialogViewer
                            .CloseDialogAsync(DialogViewLayer.Content, ct)
                            .IfSuccessAsync(
                                () =>
                                    toDoService.AddRootToDoItemAsync(
                                        vm.ToAddRootToDoItemOptions(),
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
                    .IfSuccessAsync(() => navigator.NavigateToAsync<LoginViewModel>(ct), ct)
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
                                    () =>
                                        view
                                            .DataContext.ThrowIfNull()
                                            .CastObject<CreateUserViewModel>()
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
                view
                    .DataContext.ThrowIfNull()
                    .CastObject<LoginViewModel>()
                    .IfSuccessAsync(
                        viewModel =>
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
                                                                    return this.PostUiBackground(
                                                                            () =>
                                                                            {
                                                                                var textBox =
                                                                                    view.GetControl<TextBox>(
                                                                                        LoginView.LoginTextBoxName
                                                                                    );

                                                                                textBox.Focus();

                                                                                if (
                                                                                    textBox.Text
                                                                                    is null
                                                                                )
                                                                                {
                                                                                    return Result.Success;
                                                                                }

                                                                                textBox.CaretIndex =
                                                                                    textBox
                                                                                        .Text
                                                                                        .Length;

                                                                                return Result.Success;
                                                                            },
                                                                            ct
                                                                        )
                                                                        .ToValueTaskResult()
                                                                        .ConfigureAwait(false);
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
                                                                                    x.Type
                                                                                    == ClaimTypes.Name
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
                                                                                        navigator.NavigateToAsync<RootToDoItemsViewModel>(
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
                                                        view
                                                            .DataContext.ThrowIfNull()
                                                            .CastObject<LoginViewModel>()
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

        PasswordGeneratorViewInitialized = SpravyCommand.Create<PasswordGeneratorViewModel>(
            (vm, ct) => vm.RefreshAsync(ct),
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

        MultiToDoItemsViewInitialized = SpravyCommand.Create<MultiToDoItemsView>(
            (view, _) =>
                view
                    .DataContext.ThrowIfNull()
                    .CastObject<MultiToDoItemsViewModel>()
                    .IfSuccess(vm =>
                    {
                        vm.MultiToDoItemsView = view;

                        return Result.Success;
                    })
                    .ToValueTaskResult()
                    .ConfigureAwait(false),
            errorHandler,
            taskProgressService
        );

        CopyToClipboard = SpravyCommand.Create<string>(
            (str, _) => clipboardService.SetTextAsync(str),
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
    public SpravyCommand AddRootToDoItemViewInitialized { get; }
    public SpravyCommand MultiToDoItemsViewInitialized { get; }

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
                                                        navigator.NavigateToAsync<RootToDoItemsViewModel>(
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
