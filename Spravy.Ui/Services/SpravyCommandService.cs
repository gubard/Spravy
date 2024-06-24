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
        SukiTheme sukiTheme,
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
            errorHandler, taskProgressService);

        MultiClone = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                        itemNotify => dialogViewer.CloseInputDialogAsync(ct)
                           .IfSuccessAsync(() => selected.ToResult(), ct)
                           .IfSuccessForEachAsync(
                                i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(), ct),
                                ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct), ActionHelper<ToDoItemSelectorViewModel>.Empty, ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<ResetToDoItemViewModel>.Empty,
                ct), errorHandler, taskProgressService);

        MultiReset = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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

                    return dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(() => selected.ToResult(), ct)
                       .IfSuccessForEachAsync(
                            i => toDoService.ResetToDoItemAsync(
                                new(i, vm.IsCompleteChildrenTask, vm.IsMoveCircleOrderIndex, vm.IsOnlyCompletedTasks,
                                    vm.IsCompleteCurrentTask), ct), ct)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                            ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<ResetToDoItemViewModel>.Empty,
                ct), errorHandler, taskProgressService);

        MultiChangeOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                        selectedItem => dialogViewer.CloseContentDialogAsync(ct)
                           .IfSuccessAsync(() => selected.ToResult(), ct)
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemOrderIndexAsync(
                                    new(i, selectedItem.Id, viewModel.IsAfter), ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct), ct),
                _ => dialogViewer.CloseContentDialogAsync(ct), viewModel =>
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
                }, ct);
        }, errorHandler, taskProgressService);

        MultiRandomizeChildrenOrder = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                _ => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => selected.ToResult(), ct)
                   .IfSuccessForEachAsync(i => toDoService.RandomizeChildrenOrderIndexAsync(i, ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct), viewModel =>
                {
                    viewModel.RandomizeChildrenOrderIds = selected;
                }, ct);
        }, errorHandler, taskProgressService);

        MultiCopyToClipboard = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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

                    return dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(() => selected.ToResult(), ct)
                       .IfSuccessForEachAsync(
                            i => toDoService.ToDoItemToStringAsync(new(statuses, i), ct),
                            ct)
                       .IfSuccessAsync(
                            items => clipboardService.SetTextAsync(items.Join(Environment.NewLine).ToString()),
                            ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, ct);
        }, errorHandler, taskProgressService);

        MultiMakeAsRoot = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
               .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, ct), ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiChangeParent = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                vm => dialogViewer.CloseInputDialogAsync(ct)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemParentAsync(i, vm.Id, ct),
                                ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), viewModel =>
                {
                    viewModel.IgnoreIds = selected;
                }, ct);
        }, errorHandler, taskProgressService);

        MultiOpenLeaf = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
            }, ct);
        }, errorHandler, taskProgressService);

        MultiShowSetting = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                   .CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => selected.ToResult()
                       .IfSuccessForEachAsync(item => Result.AwaitableSuccess
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsLink)
                                {
                                    return toDoService.UpdateToDoItemLinkAsync(item.Id, vm.Link.ToOptionUri(),
                                        ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsName)
                                {
                                    return toDoService.UpdateToDoItemNameAsync(item.Id, vm.Name, ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsType)
                                {
                                    return toDoService.UpdateToDoItemTypeAsync(item.Id, vm.Type, ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct), ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<MultiToDoItemSettingViewModel>.Empty, ct);
        }, errorHandler, taskProgressService);

        MultiDelete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                _ => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(item => toDoService.DeleteToDoItemAsync(item.Id, ct),
                                ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct), vm =>
                {
                    vm.DeleteItems.Update(selected);
                }, ct);
        }, errorHandler, taskProgressService);

        MultiAddChild = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                                options => dialogViewer.CloseContentDialogAsync(ct)
                                   .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, ct),
                                        ct)
                                   .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                        ct), ct), ct),
                _ => dialogViewer.CloseContentDialogAsync(ct), ActionHelper<AddToDoItemViewModel>.Empty,
                ct);
        }, errorHandler, taskProgressService);

        MultiOpenLink = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                       .IfSuccessAsync(link => openerLink.OpenLinkAsync(link.ToUri(), ct),
                            ct), ct);
        }, errorHandler, taskProgressService);

        MultiRemoveFromFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
               .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, ct),
                    ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiAddToFavorite = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
               .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, ct),
                    ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiComplete = SpravyCommand.Create<IToDoSubItemsViewModelProperty>((view, ct) =>
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
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, true, ct);
                        case ToDoItemIsCan.CanIncomplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, false, ct);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(i.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        NavigateToCurrentToDoItem = SpravyCommand.Create(ct => toDoService
           .GetCurrentActiveToDoItemAsync(ct)
           .IfSuccessAsync(activeToDoItem =>
            {
                if (activeToDoItem.TryGetValue(out var value))
                {
                    return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = value.Id,
                        ct);
                }

                return navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, ct);
            }, ct), errorHandler, taskProgressService);

        Back = SpravyCommand.Create(
            ct => navigator.NavigateBackAsync(ct).ToResultOnlyAsync(), errorHandler,
            taskProgressService);

        SendNewVerificationCode = SpravyCommand.Create<IVerificationEmail>((verificationEmail, ct) =>
        {
            switch (verificationEmail.IdentifierType)
            {
                case UserIdentifierType.Email:
                    return authenticationService.UpdateVerificationCodeByEmailAsync(verificationEmail.Identifier,
                        ct);
                case UserIdentifierType.Login:
                    return authenticationService.UpdateVerificationCodeByLoginAsync(verificationEmail.Identifier,
                        ct);
                default:
                    return new Result(new UserIdentifierTypeOutOfRangeError(verificationEmail.IdentifierType))
                       .ToValueTaskResult()
                       .ConfigureAwait(false);
            }
        }, errorHandler, taskProgressService);

        GeneratePassword = SpravyCommand.Create<PasswordItemNotify>((passwordItem, ct) =>
        {
            return passwordService.GeneratePasswordAsync(passwordItem.Id, ct)
               .IfSuccessAsync(clipboardService.SetTextAsync, ct)
               .IfSuccessAsync(
                    () => spravyNotificationManager.ShowAsync(
                        new TextLocalization("PasswordGeneratorView.Notification.CopyPassword", passwordItem),
                        ct), ct);
        }, errorHandler, taskProgressService);

        DeletePasswordItem = SpravyCommand.Create<PasswordItemNotify>(
            (passwordItem, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync<DeletePasswordItemViewModel>(
                    _ => dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(
                            () => passwordService.DeletePasswordItemAsync(passwordItem.Id, ct),
                            ct)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                            ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                    view => view.PasswordItemId = passwordItem.Id, ct), errorHandler,
            taskProgressService);

        Complete = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            Result.AwaitableSuccess
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
                               .IfErrorsAsync(_ => Result.Success, ct)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(item.CurrentId, true,
                                        ct), ct);
                        case ToDoItemIsCan.CanIncomplete:
                            return this.InvokeUiBackgroundAsync(() =>
                                {
                                    item.IsCan = ToDoItemIsCan.None;
                                    item.Status = ToDoItemStatus.ReadyForComplete;

                                    return uiApplicationService.GetCurrentView<IToDoItemUpdater>()
                                       .IfSuccess(u => u.UpdateInListToDoItemUi(item));
                                })
                               .IfErrorsAsync(_ => Result.Success, ct)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(item.CurrentId, false,
                                        ct), ct);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(item.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);

            return Result.AwaitableSuccess;
        }, errorHandler, taskProgressService);

        AddChild = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                viewModel => viewModel.ConverterToAddToDoItemOptions()
                   .IfSuccessAsync(
                        options => dialogViewer.CloseContentDialogAsync(ct)
                           .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, ct),
                                ct)
                           .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct), ct),
                _ => dialogViewer.CloseContentDialogAsync(ct), vm => vm.ParentId = item.CurrentId,
                ct), errorHandler, taskProgressService);

        Delete = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(_ => dialogViewer
                   .CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(item.Id, ct), ct)
                   .IfSuccessAsync(uiApplicationService.GetCurrentViewType, ct)
                   .IfSuccessAsync(type =>
                    {
                        if (type != typeof(ToDoItemViewModel))
                        {
                            return Result.AwaitableSuccess;
                        }

                        if (item.Parent is null)
                        {
                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(ct);
                        }

                        return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = item.Parent.Id,
                            ct);
                    }, ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                view => view.Item = item, ct), errorHandler, taskProgressService);

        ShowSetting = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
                vm => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemNameAsync(item.Id, vm.ToDoItemContent.Name, ct),
                        ct)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemLinkAsync(item.Id, vm.ToDoItemContent.Link.ToOptionUri(),
                            ct), ct)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemTypeAsync(item.Id, vm.ToDoItemContent.Type, ct),
                        ct)
                   .IfSuccessAsync(() => vm.Settings.ThrowIfNull().ApplySettingsAsync(ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                vm => vm.ToDoItemId = item.Id, ct), errorHandler, taskProgressService);

        OpenLeaf = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) =>
                navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Item = item, ct),
            errorHandler, taskProgressService);

        ChangeParent = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
            dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                i => dialogViewer.CloseInputDialogAsync(ct)
                   .IfSuccessAsync(() => toDoService.UpdateToDoItemParentAsync(item.Id, i.Id, ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), viewModel =>
                {
                    viewModel.IgnoreIds = new([item.Id,]);
                    viewModel.DefaultSelectedItemId = (item.Parent?.Id).GetValueOrDefault();
                }, ct), errorHandler, taskProgressService);

        MakeAsRoot = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => toDoService.ToDoItemToRootAsync(item.Id, ct)
               .IfSuccessAsync(
                    () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, ct),
                    ct), errorHandler, taskProgressService);

        CopyToClipboard = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
                dialogViewer.ShowConfirmContentDialogAsync(view =>
                    {
                        var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                        var options = new ToDoItemToStringOptions(statuses, item.CurrentId);

                        return dialogViewer.CloseContentDialogAsync(ct)
                           .IfSuccessAsync(
                                () => toDoService.ToDoItemToStringAsync(options, ct)
                                   .IfSuccessAsync(clipboardService.SetTextAsync, ct),
                                ct);
                    }, _ => dialogViewer.CloseContentDialogAsync(ct),
                    ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, ct), errorHandler,
            taskProgressService);

        RandomizeChildrenOrder = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => toDoService.RandomizeChildrenOrderIndexAsync(item.CurrentId, ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                viewModel => viewModel.Item = item, ct), errorHandler, taskProgressService);

        ChangeOrder = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
            dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(viewModel =>
                {
                    var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                    var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);

                    return dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(() => toDoService.UpdateToDoItemOrderIndexAsync(options, ct),
                            ct)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                            ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct), viewModel => viewModel.Id = item.Id,
                ct), errorHandler, taskProgressService);

        Reset = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                vm => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => toDoService.ResetToDoItemAsync(vm.ToResetToDoItemOptions(), ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                vm => vm.Id = item.CurrentId, ct), errorHandler, taskProgressService);

        AddToFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => toDoService.AddFavoriteToDoItemAsync(item.Id, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct), errorHandler, taskProgressService);

        RemoveFromFavorite = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => toDoService.RemoveFavoriteToDoItemAsync(item.Id, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct), errorHandler, taskProgressService);

        OpenLink = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            var link = item.Link.ThrowIfNull().ToUri();

            return openerLink.OpenLinkAsync(link, ct);
        }, errorHandler, taskProgressService);

        Clone = SpravyCommand.Create<ToDoItemEntityNotify>(
            (item, ct) => dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                itemNotify => dialogViewer.CloseInputDialogAsync(ct)
                   .IfSuccessAsync(
                        () => toDoService.CloneToDoItemAsync(item.Id, itemNotify.Id.ToOption(), ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), view => view.DefaultSelectedItemId = item.Id, ct),
            errorHandler, taskProgressService);

        MultiAddChildToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                viewModel => viewModel.ConverterToAddToDoItemOptions()
                   .IfSuccessAsync(
                        options => dialogViewer.CloseContentDialogAsync(ct)
                           .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, ct),
                                ct)
                           .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct), ct),
                _ => dialogViewer.CloseContentDialogAsync(ct), vm => vm.ParentId = item.CurrentId,
                ct);
        }, errorHandler, taskProgressService);

        MultiShowSettingToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<MultiToDoItemSettingViewModel>(vm => dialogViewer
                   .CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => selected.ToResult()
                       .IfSuccessForEachAsync(i => Result.AwaitableSuccess
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsLink)
                                {
                                    return toDoService.UpdateToDoItemLinkAsync(i.Id, vm.Link.ToOptionUri(),
                                        ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsName)
                                {
                                    return toDoService.UpdateToDoItemNameAsync(i.Id, vm.Name, ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct)
                           .IfSuccessAsync(() =>
                            {
                                if (vm.IsType)
                                {
                                    return toDoService.UpdateToDoItemTypeAsync(i.Id, vm.Type, ct);
                                }

                                return Result.AwaitableSuccess;
                            }, ct), ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                vm => vm.ToDoItemId = item.Id, ct);
        }, errorHandler, taskProgressService);

        MultiDeleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = item.Children.Where(x => x.IsSelected).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(i => toDoService.DeleteToDoItemAsync(i.Id, ct),
                                ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct), vm =>
                {
                    vm.Item = item;
                    vm.DeleteItems.Update(selected);
                }, ct);
        }, errorHandler, taskProgressService);

        MultiOpenLeafToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
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
            }, ct);
        }, errorHandler, taskProgressService);

        MultiChangeParentToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                vm => dialogViewer.CloseInputDialogAsync(ct)
                   .IfSuccessAsync(
                        () => selected.ToResult()
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemParentAsync(i, vm.Id, ct),
                                ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), viewModel =>
                {
                    viewModel.IgnoreIds = selected;
                    viewModel.DefaultSelectedItemId = item.Id;
                }, ct);
        }, errorHandler, taskProgressService);

        MultiMakeAsRootToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, ct), ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiCopyToClipboardToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync(view =>
                {
                    var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);

                    return dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(() => selected.ToResult(), ct)
                       .IfSuccessForEachAsync(
                            i => toDoService.ToDoItemToStringAsync(new(statuses, i), ct),
                            ct)
                       .IfSuccessAsync(
                            items => clipboardService.SetTextAsync(items.Join(Environment.NewLine).ToString()),
                            ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, ct);
        }, errorHandler, taskProgressService);

        MultiRandomizeChildrenOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => selected.ToResult(), ct)
                   .IfSuccessForEachAsync(i => toDoService.RandomizeChildrenOrderIndexAsync(i, ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct), viewModel =>
                {
                    viewModel.Item = item;
                    viewModel.RandomizeChildrenOrderIds = selected;
                }, ct);
        }, errorHandler, taskProgressService);

        MultiChangeOrderToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
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
                        selectedItem => dialogViewer.CloseContentDialogAsync(ct)
                           .IfSuccessAsync(() => selected.ToResult(), ct)
                           .IfSuccessForEachAsync(
                                i => toDoService.UpdateToDoItemOrderIndexAsync(
                                    new(i, selectedItem.Id, viewModel.IsAfter), ct), ct)
                           .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                                ct), ct),
                _ => dialogViewer.CloseContentDialogAsync(ct), viewModel =>
                {
                    viewModel.Id = item.Id;

                    viewModel.ChangeToDoItemOrderIndexIds =
                        item.Children.Where(x => !x.IsSelected).Select(x => x.Id).ToArray();
                }, ct);
        }, errorHandler, taskProgressService);

        MultiResetToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
            dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(vm =>
                {
                    ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                    if (selected.IsEmpty)
                    {
                        return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                    }

                    return dialogViewer.CloseContentDialogAsync(ct)
                       .IfSuccessAsync(() => selected.ToResult(), ct)
                       .IfSuccessForEachAsync(
                            i => toDoService.ResetToDoItemAsync(
                                new(i, vm.IsCompleteChildrenTask, vm.IsMoveCircleOrderIndex, vm.IsOnlyCompletedTasks,
                                    vm.IsCompleteCurrentTask), ct), ct)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                            ct);
                }, _ => dialogViewer.CloseContentDialogAsync(ct), vm => vm.Id = item.CurrentId,
                ct), errorHandler, taskProgressService);

        MultiCloneToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                itemNotify => dialogViewer.CloseInputDialogAsync(ct)
                   .IfSuccessAsync(() => selected.ToResult(), ct)
                   .IfSuccessForEachAsync(
                        i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(), ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), view => view.DefaultSelectedItemId = item.Id, ct);
        }, errorHandler, taskProgressService);

        MultiOpenLinkToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
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
                       .IfSuccessAsync(link => openerLink.OpenLinkAsync(link.ToUri(), ct),
                            ct), ct);
        }, errorHandler, taskProgressService);

        MultiAddToFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, ct),
                    ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiRemoveFromFavoriteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
        {
            ReadOnlyMemory<Guid> selected = item.Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, ct),
                    ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        MultiCompleteToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
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
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, true, ct);
                        case ToDoItemIsCan.CanIncomplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(i.Id, false, ct);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(i.IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, ct)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                    ct);
        }, errorHandler, taskProgressService);

        SwitchPane = SpravyCommand.Create(ct => ct.InvokeUiBackgroundAsync(() =>
        {
            mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen;

            return Result.Success;
        }), errorHandler, taskProgressService);

        AddRootToDoItem = SpravyCommand.Create(ct => dialogViewer.ShowConfirmContentDialogAsync(view =>
            {
                var options = view.ToAddRootToDoItemOptions();

                return dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => toDoService.AddRootToDoItemAsync(options, ct),
                        ct)
                   .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct);
            }, _ => dialogViewer.CloseContentDialogAsync(ct),
            ActionHelper<AddRootToDoItemViewModel>.Empty,
            ct), errorHandler, taskProgressService);

        Logout = SpravyCommand.Create(ct => objectStorage.IsExistsAsync(StorageIds.LoginId, ct)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return objectStorage.DeleteAsync(StorageIds.LoginId, ct);
                }

                return Result.AwaitableSuccess;
            }, ct)
           .IfSuccessAsync(() => navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, ct),
                ct)
           .IfSuccessAsync(() => ct.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.IsPaneOpen = false;

                return Result.Success;
            }), ct), errorHandler, taskProgressService);

        RefreshCurrentView = SpravyCommand.Create(uiApplicationService.RefreshCurrentViewAsync, errorHandler,
            taskProgressService);

        SetToDoItemDescription = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) =>
            dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
                viewModel => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemDescriptionAsync(item.Id, viewModel.Content.Description,
                            ct), ct)
                   .IfSuccessAsync(
                        () => toDoService.UpdateToDoItemDescriptionTypeAsync(item.Id, viewModel.Content.Type,
                            ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct), viewModel =>
                {
                    viewModel.Content.Description = item.Description;
                    viewModel.Content.Type = item.DescriptionType;
                    viewModel.ToDoItemName = item.Name;
                }, ct), errorHandler, taskProgressService);

        AddPasswordItem = SpravyCommand.Create(
            ct => dialogViewer.ShowConfirmContentDialogAsync(
                vm => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(
                        () => passwordService.AddPasswordItemAsync(vm.ToAddPasswordOptions(), ct),
                        ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                ActionHelper<AddPasswordItemViewModel>.Empty, ct), errorHandler, taskProgressService);

        ShowPasswordItemSetting = SpravyCommand.Create<PasswordItemNotify>(
            (item, ct) => dialogViewer.ShowConfirmContentDialogAsync<PasswordItemSettingsViewModel>(
                vm => dialogViewer.CloseContentDialogAsync(ct)
                   .IfSuccessAsync(() => passwordService.UpdatePasswordItemKeyAsync(item.Id, vm.Key, ct),
                        ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemLengthAsync(item.Id, vm.Length, ct),
                        ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemNameAsync(item.Id, vm.Name, ct),
                        ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemRegexAsync(item.Id, vm.Regex, ct),
                        ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemCustomAvailableCharactersAsync(item.Id,
                            vm.CustomAvailableCharacters, ct), ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemIsAvailableNumberAsync(item.Id, vm.IsAvailableNumber,
                            ct), ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemIsAvailableLowerLatinAsync(item.Id,
                            vm.IsAvailableLowerLatin, ct), ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(item.Id,
                            vm.IsAvailableSpecialSymbols, ct), ct)
                   .IfSuccessAsync(
                        () => passwordService.UpdatePasswordItemIsAvailableUpperLatinAsync(item.Id,
                            vm.IsAvailableUpperLatin, ct), ct)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(ct),
                        ct), _ => dialogViewer.CloseContentDialogAsync(ct),
                vm => vm.Id = item.Id, ct), errorHandler, taskProgressService);

        SettingViewInitialized = SpravyCommand.Create<SettingViewModel>((viewModel, ct) =>
            this.InvokeUiBackgroundAsync(() =>
            {
                viewModel.AvailableColors.Clear();
                viewModel.AvailableColors.AddRange(sukiTheme.ColorThemes.Select(x => new Selected<SukiColorTheme>(x)));

                foreach (var availableColor in viewModel.AvailableColors)
                {
                    if (availableColor.Value == sukiTheme.ActiveColorTheme)
                    {
                        availableColor.IsSelect = true;
                    }
                }

                viewModel.IsLightTheme = sukiTheme.ActiveBaseTheme == ThemeVariant.Light;

                return Result.Success;
            }), errorHandler, taskProgressService);

        NavigateToActiveToDoItem = SpravyCommand.Create<ToDoItemEntityNotify>((item, ct) => toDoService
           .GetActiveToDoItemAsync(item.Id, ct)
           .IfSuccessAsync(active =>
            {
                if (active.TryGetValue(out var value))
                {
                    if (value.ParentId.TryGetValue(out var parentId))
                    {
                        return navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = parentId, ct);
                    }

                    return navigator.NavigateToAsync<RootToDoItemsViewModel>(ct);
                }

                return uiApplicationService.RefreshCurrentViewAsync(ct);
            }, ct), errorHandler, taskProgressService);
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