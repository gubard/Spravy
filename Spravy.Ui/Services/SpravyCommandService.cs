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
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IAuthenticationService authenticationService,
        IPasswordService passwordService,
        MainSplitViewModel mainSplitViewModel,
        IObjectStorage objectStorage,
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
            (item, cancellationToken) =>
                navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = item.CurrentId, cancellationToken),
            errorHandler, taskProgressService);

        MultiClone = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync(vm =>
                {
                    var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

                    if (view.IsHasError)
                    {
                        return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    }

                    ReadOnlyMemory<Guid> selected = view.Value
                       .ToDoSubItemsViewModel
                       .List
                       .ToDoItems
                       .GroupByNone
                       .Items
                       .Items
                       .Where(x => x.IsSelected)
                       .Select(x => x.Id)
                       .ToArray();

                    if (selected.IsEmpty)
                    {
                        return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                        itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                           .IfSuccessForEachAsync(
                                i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(), cancellationToken),
                                cancellationToken)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken), ActionHelper<ToDoItemSelectorViewModel>.Empty, cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ResetToDoItemViewModel>.Empty,
                cancellationToken), errorHandler, taskProgressService);

        MultiReset = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync(vm =>
                {
                    var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

                    if (view.IsHasError)
                    {
                        return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    }

                    ReadOnlyMemory<Guid> selected = view.Value
                       .ToDoSubItemsViewModel
                       .List
                       .ToDoItems
                       .GroupByNone
                       .Items
                       .Items
                       .Where(x => x.IsSelected)
                       .Select(x => x.Id)
                       .ToArray();

                    if (selected.IsEmpty)
                    {
                        return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                       .IfSuccessForEachAsync(
                            i => toDoService.ResetToDoItemAsync(
                                new(i, vm.IsCompleteChildrenTask, vm.IsMoveCircleOrderIndex, vm.IsOnlyCompletedTasks,
                                    vm.IsCompleteCurrentTask), cancellationToken), cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ResetToDoItemViewModel>.Empty,
                cancellationToken), errorHandler, taskProgressService);

        MultiChangeOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                viewModel => viewModel.SelectedItem
                   .IfNotNull(nameof(viewModel.SelectedItem))
                   .IfSuccessAsync(
                        selectedItem => dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemOrderIndexAsync(
                                    new(i, selectedItem.Id, viewModel.IsAfter), cancellationToken), cancellationToken)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken), cancellationToken),
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.ChangeToDoItemOrderIndexIds = view.ToDoSubItemsViewModel
                       .List
                       .ToDoItems
                       .GroupByNone
                       .Items
                       .Items
                       .Where(x => !x.IsSelected)
                       .Select(x => x.Id)
                       .ToArray();
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiRandomizeChildrenOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                   .IfSuccessForEachAsync(i => toDoService.RandomizeChildrenOrderIndexAsync(i, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.RandomizeChildrenOrderIds = selected;
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiCopyToClipboard = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync(view =>
                {
                    var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                       .IfSuccessForEachAsync(
                            i => toDoService.ToDoItemToStringAsync(new(statuses, i), cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(
                            items => clipboardService.SetTextAsync(items.Join(Environment.NewLine).ToString()),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiMakeAsRoot = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiChangeParent = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                vm => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemParentAsync(i, vm.Id, cancellationToken),
                                cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), viewModel =>
                {
                    viewModel.IgnoreIds = selected;
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiOpenLeaf = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm =>
            {
                vm.LeafIds = selected;
            }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiShowSetting = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync(vm => dialogViewer
                   .CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult()
                       .IfSuccessForEachAsync(item => Result.AwaitableSuccess
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsLink)
                                {
                                    return toDoService.UpdateToDoItemLinkAsync(item.Id, vm.Link.ToOptionUri(),
                                        cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsName)
                                {
                                    return toDoService.UpdateToDoItemNameAsync(item.Id, vm.Name, cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsType)
                                {
                                    return toDoService.UpdateToDoItemTypeAsync(item.Id, vm.Type, cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken), cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<MultiToDoItemSettingViewModel>.Empty, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiDelete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(item => toDoService.DeleteToDoItemAsync(item.Id, cancellationToken),
                                cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm =>
                {
                    vm.DeleteItems.Update(selected);
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiAddChild = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync(
                viewModel => selected.ToResult()
                   .IfSuccessForEachAsync(
                        item => viewModel.ConverterToAddToDoItemOptions(item.Id)
                           .IfSuccessAsync(
                                options => dialogViewer.CloseContentDialogAsync(cancellationToken)
                                   .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                        cancellationToken)
                                   .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                        cancellationToken), cancellationToken), cancellationToken),
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<AddToDoItemViewModel>.Empty,
                cancellationToken);
        }, errorHandler, taskProgressService);

        MultiOpenLink = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(
                    i => i.Link
                       .IfNotNull(nameof(i.Link))
                       .IfSuccessAsync(link => openerLink.OpenLinkAsync(link.ToUri(), cancellationToken),
                            cancellationToken), cancellationToken);
        }, errorHandler, taskProgressService);

        MultiRemoveFromFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiAddToFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .Select(x => x.Id)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiComplete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.ToDoSubItemsViewModel
               .List
               .ToDoItems
               .GroupByNone
               .Items
               .Items
               .Where(x => x.IsSelected)
               .ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i =>
                {
                    switch (i.IsCan)
                    {
                        case ToDoItemIsCan.None:
                            return Result.AwaitableSuccess;
                        case ToDoItemIsCan.CanComplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, true, cancellationToken);
                        case ToDoItemIsCan.CanIncomplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, false, cancellationToken);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(i.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        NavigateToCurrentToDoItem = SpravyCommand.Create(cancellationToken => toDoService
           .GetCurrentActiveToDoItemAsync(cancellationToken)
           .IfSuccessAsync(activeToDoItem =>
            {
                if (activeToDoItem.TryGetValue(out var value))
                {
                    return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = value.Id,
                        cancellationToken);
                }

                return navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
            }, cancellationToken), errorHandler, taskProgressService);

        Back = SpravyCommand.Create(
            cancellationToken => navigator.NavigateBackAsync(cancellationToken).ToResultOnlyAsync(), errorHandler,
            taskProgressService);

        SendNewVerificationCode = SpravyCommand.Create<IVerificationEmail>((verificationEmail, cancellationToken) =>
        {
            switch (verificationEmail.IdentifierType)
            {
                case UserIdentifierType.Email:
                    return authenticationService.UpdateVerificationCodeByEmailAsync(verificationEmail.Identifier,
                        cancellationToken);
                case UserIdentifierType.Login:
                    return authenticationService.UpdateVerificationCodeByLoginAsync(verificationEmail.Identifier,
                        cancellationToken);
                default:
                    return new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                       .ToValueTaskResult()
                       .ConfigureAwait(false);
            }
        }, errorHandler, taskProgressService);

        GeneratePassword = SpravyCommand.Create<PasswordItemNotify>((passwordItem, cancellationToken) =>
        {
            return passwordService.GeneratePasswordAsync(passwordItem.Id, cancellationToken)
               .IfSuccessAsync(clipboardService.SetTextAsync, cancellationToken)
               .IfSuccessAsync(
                    () => spravyNotificationManager.ShowAsync(
                        new TextLocalization("PasswordGeneratorView.Notification.CopyPassword", passwordItem),
                        cancellationToken), cancellationToken);
        }, errorHandler, taskProgressService);

        DeletePasswordItem = SpravyCommand.Create<PasswordItemNotify>(
            (passwordItem, cancellationToken) =>
                dialogViewer.ShowConfirmContentDialogAsync<DeletePasswordItemViewModel>(
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => passwordService.DeletePasswordItemAsync(passwordItem.Id, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    view => view.PasswordItemId = passwordItem.Id, cancellationToken), errorHandler,
            taskProgressService);

        Complete = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            return Result.AwaitableSuccess
               .IfSuccessAsync(() =>
                {
                    switch (item.IsCan)
                    {
                        case ToDoItemIsCan.None:
                            return Result.AwaitableSuccess;
                        case ToDoItemIsCan.CanComplete:
                            return this.InvokeUiBackgroundAsync(() =>
                                {
                                    item.IsCan = ToDoItemIsCan.None;
                                    item.Status = ToDoItemStatus.Completed;

                                    return uiApplicationService.GetCurrentView<IToDoItemUpdater>()
                                       .IfSuccess(u => u.UpdateInListToDoItemUi(item));
                                })
                               .IfErrorsAsync(_ => Result.Success, cancellationToken)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(item.CurrentId, true,
                                        cancellationToken), cancellationToken);
                        case ToDoItemIsCan.CanIncomplete:
                            return this.InvokeUiBackgroundAsync(() =>
                                {
                                    item.IsCan = ToDoItemIsCan.None;
                                    item.Status = ToDoItemStatus.ReadyForComplete;

                                    return uiApplicationService.GetCurrentView<IToDoItemUpdater>()
                                       .IfSuccess(u => u.UpdateInListToDoItemUi(item));
                                })
                               .IfErrorsAsync(_ => Result.Success, cancellationToken)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(item.CurrentId, false,
                                        cancellationToken), cancellationToken);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(item.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        AddChild = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                viewModel => viewModel.ConverterToAddToDoItemOptions()
                   .IfSuccessAsync(
                        options => dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                cancellationToken)
                           .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken), cancellationToken),
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ParentId = item.CurrentId,
                cancellationToken), errorHandler, taskProgressService);

        Delete = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(_ => dialogViewer
                   .CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(item.Id, cancellationToken), cancellationToken)
                   .IfSuccessAsync(uiApplicationService.GetCurrentViewType, cancellationToken)
                   .IfSuccessAsync(type =>
                    {
                        if (type != typeof(ToDoItemViewModel))
                        {
                            return Result.AwaitableSuccess;
                        }

                        if (item.Parent is null)
                        {
                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken);
                        }

                        return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = item.Parent.Id,
                            cancellationToken);
                    }, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Item = item, cancellationToken), errorHandler, taskProgressService);

        ShowSetting = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
                vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemNameAsync(item.Id, vm.ToDoItemContent.Name, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemLinkAsync(item.Id, vm.ToDoItemContent.Link.ToOptionUri(),
                            cancellationToken), cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemTypeAsync(item.Id, vm.ToDoItemContent.Type, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => vm.Settings.ThrowIfNull().ApplySettingsAsync(cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                vm => vm.ToDoItemId = item.Id, cancellationToken), errorHandler, taskProgressService);

        OpenLeaf = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) =>
                navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Item = item, cancellationToken),
            errorHandler, taskProgressService);

        ChangeParent = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                i => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.UpdateToDoItemParentAsync(item.Id, i.Id, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), viewModel =>
                {
                    viewModel.IgnoreIds = new([item.Id,]);
                    viewModel.DefaultSelectedItemId = (item.Parent?.Id).GetValueOrDefault();
                }, cancellationToken), errorHandler, taskProgressService);

        MakeAsRoot = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => toDoService.ToDoItemToRootAsync(item.Id, cancellationToken)
               .IfSuccessAsync(
                    () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken),
                    cancellationToken), errorHandler, taskProgressService);

        CopyToClipboard = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
                dialogViewer.ShowConfirmContentDialogAsync(view =>
                    {
                        var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                        var options = new ToDoItemToStringOptions(statuses, item.CurrentId);

                        return dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(
                                () => toDoService.ToDoItemToStringAsync(options, cancellationToken)
                                   .IfSuccessAsync(clipboardService.SetTextAsync, cancellationToken),
                                cancellationToken);
                    }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken), errorHandler,
            taskProgressService);

        RandomizeChildrenOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.RandomizeChildrenOrderIndexAsync(item.CurrentId, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                viewModel => viewModel.Item = item, cancellationToken), errorHandler, taskProgressService);

        ChangeOrder = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(viewModel =>
                {
                    var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                    var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => toDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel => viewModel.Id = item.Id,
                cancellationToken), errorHandler, taskProgressService);

        Reset = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.ResetToDoItemAsync(vm.ToResetToDoItemOptions(), cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                vm => vm.Id = item.CurrentId, cancellationToken), errorHandler, taskProgressService);

        AddToFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => toDoService.AddFavoriteToDoItemAsync(item.Id, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken), errorHandler, taskProgressService);

        RemoveFromFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => toDoService.RemoveFavoriteToDoItemAsync(item.Id, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken), errorHandler, taskProgressService);

        OpenLink = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            var link = item.Link.ThrowIfNull().ToUri();

            return openerLink.OpenLinkAsync(link, cancellationToken);
        }, errorHandler, taskProgressService);

        Clone = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, cancellationToken) => dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.CloneToDoItemAsync(item.Id, itemNotify.Id.ToOption(), cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), view => view.DefaultSelectedItemId = item.Id, cancellationToken),
            errorHandler, taskProgressService);

        MultiAddChildToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                viewModel => viewModel.ConverterToAddToDoItemOptions()
                   .IfSuccessAsync(
                        options => dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                cancellationToken)
                           .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken), cancellationToken),
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ParentId = item.CurrentId,
                cancellationToken);
        }, errorHandler, taskProgressService);

        MultiShowSettingToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<MultiToDoItemSettingViewModel>(vm => dialogViewer
                   .CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult()
                       .IfSuccessForEachAsync(i => Result.AwaitableSuccess
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsLink)
                                {
                                    return toDoService.UpdateToDoItemLinkAsync(i.Id, vm.Link.ToOptionUri(),
                                        cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsName)
                                {
                                    return toDoService.UpdateToDoItemNameAsync(i.Id, vm.Name, cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsType)
                                {
                                    return toDoService.UpdateToDoItemTypeAsync(i.Id, vm.Type, cancellationToken);
                                }

                                return Result.AwaitableSuccess;
                            }, cancellationToken), cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                vm => vm.ToDoItemId = item.Id, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiDeleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(i => toDoService.DeleteToDoItemAsync(i.Id, cancellationToken),
                                cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm =>
                {
                    vm.Item = item;
                    vm.DeleteItems.Update(selected);
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiOpenLeafToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm =>
            {
                vm.Item = item;
                vm.LeafIds = selected;
            }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiChangeParentToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                vm => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemParentAsync(i, vm.Id, cancellationToken),
                                cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), viewModel =>
                {
                    viewModel.IgnoreIds = selected;
                    viewModel.DefaultSelectedItemId = item.Id;
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiMakeAsRootToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiCopyToClipboardToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync(view =>
                {
                    var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                       .IfSuccessForEachAsync(
                            i => toDoService.ToDoItemToStringAsync(new(statuses, i), cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(
                            items => clipboardService.SetTextAsync(items.Join(Environment.NewLine).ToString()),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiRandomizeChildrenOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                   .IfSuccessForEachAsync(i => toDoService.RandomizeChildrenOrderIndexAsync(i, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.Item = item;
                    viewModel.RandomizeChildrenOrderIds = selected;
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiChangeOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected =
                item.Children.Where(x => x.IsSelected).Select(x => x.Id).Reverse().ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                viewModel => viewModel.SelectedItem
                   .IfNotNull(nameof(viewModel.SelectedItem))
                   .IfSuccessAsync(
                        selectedItem => dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemOrderIndexAsync(
                                    new(i, selectedItem.Id, viewModel.IsAfter), cancellationToken), cancellationToken)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken), cancellationToken),
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.Id = item.Id;

                    viewModel.ChangeToDoItemOrderIndexIds =
                        item.Children.Where(x => !x.IsSelected).Select(x => x.Id).ToArray();
                }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiResetToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(vm =>
                {
                    ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                    if (selected.IsEmpty)
                    {
                        return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                       .IfSuccessForEachAsync(
                            i => toDoService.ResetToDoItemAsync(
                                new(i, vm.IsCompleteChildrenTask, vm.IsMoveCircleOrderIndex, vm.IsOnlyCompletedTasks,
                                    vm.IsCompleteCurrentTask), cancellationToken), cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.Id = item.CurrentId,
                cancellationToken), errorHandler, taskProgressService);

        MultiCloneToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                   .IfSuccessForEachAsync(
                        i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(), cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), view => view.DefaultSelectedItemId = item.Id, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiOpenLinkToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(
                    i => i.Link
                       .IfNotNull(nameof(i.Link))
                       .IfSuccessAsync(link => openerLink.OpenLinkAsync(link.ToUri(), cancellationToken),
                            cancellationToken), cancellationToken);
        }, errorHandler, taskProgressService);

        MultiAddToFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiRemoveFromFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiCompleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i =>
                {
                    switch (i.IsCan)
                    {
                        case ToDoItemIsCan.None:
                            return Result.AwaitableSuccess;
                        case ToDoItemIsCan.CanComplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, true, cancellationToken);
                        case ToDoItemIsCan.CanIncomplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, false, cancellationToken);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(i.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        SwitchPane = SpravyCommand.Create(cancellationToken => cancellationToken.InvokeUiBackgroundAsync(() =>
        {
            mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen;

            return Result.Success;
        }), errorHandler, taskProgressService);

        AddRootToDoItem = SpravyCommand.Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(view =>
            {
                var options = view.ToAddRootToDoItemOptions();

                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.AddRootToDoItemAsync(options, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<AddRootToDoItemViewModel>.Empty,
            cancellationToken), errorHandler, taskProgressService);

        Logout = SpravyCommand.Create(cancellationToken => objectStorage.IsExistsAsync(StorageIds.LoginId)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return objectStorage.DeleteAsync(StorageIds.LoginId);
                }

                return Result.AwaitableSuccess;
            }, cancellationToken)
           .IfSuccessAsync(() => navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => cancellationToken.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.IsPaneOpen = false;

                return Result.Success;
            }), cancellationToken), errorHandler, taskProgressService);

        RefreshCurrentView = SpravyCommand.Create(uiApplicationService.RefreshCurrentViewAsync, errorHandler,
            taskProgressService);

        SetToDoItemDescription = SpravyCommand.Create<ToDoItemEntityNotify>((item, cancellationToken) =>
            dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
                viewModel => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemDescriptionAsync(item.Id, viewModel.Content.Description,
                            cancellationToken), cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemDescriptionTypeAsync(item.Id, viewModel.Content.Type,
                            cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.Content.Description = item.Description;
                    viewModel.Content.Type = item.DescriptionType;
                    viewModel.ToDoItemName = item.Name;
                }, cancellationToken), errorHandler, taskProgressService);
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
    public SpravyCommand NavigateToToDoItem { get; }

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
    public SpravyCommand MultiOpenLink { get; }

    public SpravyCommand GeneratePassword { get; }
    public SpravyCommand DeletePasswordItem { get; }

    public SpravyCommand SwitchPane { get; }
    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand NavigateToCurrentToDoItem { get; }
    public SpravyCommand AddRootToDoItem { get; }
    public SpravyCommand Logout { get; }
    public SpravyCommand RefreshCurrentView { get; }
    public SpravyCommand SetToDoItemDescription { get; }

    public SpravyCommand GetNavigateTo<TViewModel>() where TViewModel : INavigatable
    {
        if (createNavigateToCache.TryGetValue(typeof(TViewModel), out var command))
        {
            return command;
        }

        var result = SpravyCommand.Create(navigator.NavigateToAsync<TViewModel>, errorHandler, taskProgressService);
        createNavigateToCache.Add(typeof(TViewModel), result);

        return result;
    }
}