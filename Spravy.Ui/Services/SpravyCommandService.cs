using Spravy.Core.Mappers;

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

        MultiClone = SpravyCommand.Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(vm =>
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
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<ResetToDoItemViewModel>.Empty,
            cancellationToken), errorHandler, taskProgressService);

        MultiReset = SpravyCommand.Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(vm =>
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
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<ResetToDoItemViewModel>.Empty,
            cancellationToken), errorHandler, taskProgressService);

        MultiChangeOrder = SpravyCommand.Create(cancellationToken =>
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
                    viewModel.ChangeToDoItemOrderIndexIds = view.Value
                       .ToDoSubItemsViewModel
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

        MultiRandomizeChildrenOrder = SpravyCommand.Create(cancellationToken =>
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

        MultiCopyToClipboard = SpravyCommand.Create(cancellationToken =>
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

        MultiMakeAsRoot = SpravyCommand.Create(cancellationToken =>
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

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiChangeParent = SpravyCommand.Create(cancellationToken =>
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

        MultiOpenLeaf = SpravyCommand.Create(cancellationToken =>
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

            return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm =>
            {
                vm.LeafIds = selected;
            }, cancellationToken);
        }, errorHandler, taskProgressService);

        MultiShowSetting = SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
            }

            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.Value
               .ToDoSubItemsViewModel
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

        MultiDelete = SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
            }

            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.Value
               .ToDoSubItemsViewModel
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

        MultiAddChild = SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
            }

            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.Value
               .ToDoSubItemsViewModel
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

        MultiOpenLink = SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
            }

            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.Value
               .ToDoSubItemsViewModel
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

        MultiRemoveFromFavorite = SpravyCommand.Create(cancellationToken =>
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

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiAddToFavorite = SpravyCommand.Create(cancellationToken =>
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

            return selected.ToResult()
               .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService);

        MultiComplete = SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<IToDoSubItemsViewModelProperty>();

            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
            }

            ReadOnlyMemory<ToDoItemEntityNotify> selected = view.Value
               .ToDoSubItemsViewModel
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
    }

    public SpravyCommand SendNewVerificationCode { get; }
    public SpravyCommand Back { get; }
    public SpravyCommand NavigateToCurrentToDoItem { get; }
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