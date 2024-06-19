using Spravy.Core.Mappers;
using Type = System.Type;

namespace Spravy.Ui.Models;

public class SpravyCommand
{
    private static readonly Dictionary<Type, SpravyCommand> createNavigateToCache;
    private static readonly Dictionary<Guid, SpravyCommand> deletePasswordItemCache;
    private static readonly Dictionary<Guid, SpravyCommand> generatePasswordCache;
    private static SpravyCommand? _sendNewVerificationCode;
    private static SpravyCommand? _back;
    private static SpravyCommand? _navigateToCurrentToDoItem;
    private static SpravyCommand? _multiComplete;
    private static SpravyCommand? _multiAddToFavorite;
    private static SpravyCommand? _multiRemoveFromFavorite;
    private static SpravyCommand? _multiAddChild;
    private static SpravyCommand? _multiDelete;
    private static SpravyCommand? _multiShowSetting;
    private static SpravyCommand? _multiOpenLeaf;
    private static SpravyCommand? _multiChangeParent;
    private static SpravyCommand? _multiMakeAsRoot;
    private static SpravyCommand? _multiCopyToClipboard;
    private static SpravyCommand? _multiRandomizeChildrenOrder;
    private static SpravyCommand? _multiChangeOrder;
    private static SpravyCommand? _multiReset;
    private static SpravyCommand? _multiClone;

    static SpravyCommand()
    {
        createNavigateToCache = new();
        deletePasswordItemCache = new();
        generatePasswordCache = new();
    }

    private SpravyCommand(TaskWork work, ICommand command)
    {
        Work = work;
        Command = command;
    }

    public TaskWork Work { get; }
    public ICommand Command { get; }

    public static SpravyCommand CreateMultiClone(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiClone is not null)
        {
            return _multiClone;
        }

        _multiClone = Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(vm =>
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
                       .IfSuccessForEachAsync(i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(), cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), ActionHelper<ToDoItemSelectorViewModel>.Empty, cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<ResetToDoItemViewModel>.Empty,
            cancellationToken), errorHandler, taskProgressService);

        return _multiClone;
    }

    public static SpravyCommand CreateMultiReset(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiReset is not null)
        {
            return _multiReset;
        }

        _multiReset = Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(vm =>
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

        return _multiReset;
    }

    public static SpravyCommand CreateMultiChangeOrder(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiChangeOrder is not null)
        {
            return _multiChangeOrder;
        }

        _multiChangeOrder = Create(cancellationToken =>
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

        return _multiChangeOrder;
    }

    public static SpravyCommand CreateMultiRandomizeChildrenOrder(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiRandomizeChildrenOrder is not null)
        {
            return _multiRandomizeChildrenOrder;
        }

        _multiRandomizeChildrenOrder = Create(cancellationToken =>
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

        return _multiRandomizeChildrenOrder;
    }

    public static SpravyCommand CreateMultiCopyToClipboard(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiCopyToClipboard is not null)
        {
            return _multiCopyToClipboard;
        }

        _multiCopyToClipboard = Create(cancellationToken =>
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

        return _multiCopyToClipboard;
    }

    public static SpravyCommand CreateMultiMakeAsRoot(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiMakeAsRoot is not null)
        {
            return _multiMakeAsRoot;
        }

        _multiMakeAsRoot = Create(cancellationToken =>
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

        return _multiMakeAsRoot;
    }

    public static SpravyCommand CreateMultiChangeParent(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiChangeParent is not null)
        {
            return _multiChangeParent;
        }

        _multiChangeParent = Create(cancellationToken =>
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

        return _multiChangeParent;
    }

    public static SpravyCommand CreateMultiOpenLeaf(
        IUiApplicationService uiApplicationService,
        INavigator navigator,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiOpenLeaf is not null)
        {
            return _multiOpenLeaf;
        }

        _multiOpenLeaf = Create(cancellationToken =>
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

        return _multiOpenLeaf;
    }

    public static SpravyCommand CreateMultiShowSetting(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiShowSetting is not null)
        {
            return _multiShowSetting;
        }

        _multiShowSetting = Create(cancellationToken =>
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

        return _multiShowSetting;
    }

    public static SpravyCommand CreateMultiDelete(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiDelete is not null)
        {
            return _multiDelete;
        }

        _multiDelete = Create(cancellationToken =>
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

        return _multiDelete;
    }

    public static SpravyCommand CreateMultiAddChild(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IDialogViewer dialogViewer,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiAddChild is not null)
        {
            return _multiAddChild;
        }

        _multiAddChild = Create(cancellationToken =>
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

        return _multiAddChild;
    }

    public static SpravyCommand CreateMultiOpenLink(
        IUiApplicationService uiApplicationService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiRemoveFromFavorite is not null)
        {
            return _multiRemoveFromFavorite;
        }

        _multiRemoveFromFavorite = Create(cancellationToken =>
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

        return _multiRemoveFromFavorite;
    }

    public static SpravyCommand CreateMultiRemoveFromFavorite(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiRemoveFromFavorite is not null)
        {
            return _multiRemoveFromFavorite;
        }

        _multiRemoveFromFavorite = Create(cancellationToken =>
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

        return _multiRemoveFromFavorite;
    }

    public static SpravyCommand CreateMultiAddToFavorite(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiAddToFavorite is not null)
        {
            return _multiAddToFavorite;
        }

        _multiAddToFavorite = Create(cancellationToken =>
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

        return _multiAddToFavorite;
    }

    public static SpravyCommand CreateMultiComplete(
        IUiApplicationService uiApplicationService,
        IToDoService toDoService,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        if (_multiComplete is not null)
        {
            return _multiComplete;
        }

        _multiComplete = Create(cancellationToken =>
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

        return _multiComplete;
    }

    public static SpravyCommand CreateNavigateToCurrentToDoItem(
        IToDoService toDoService,
        INavigator navigator,
        IErrorHandler errorHandler, 
        ITaskProgressService taskProgressService
    )
    {
        if (_navigateToCurrentToDoItem is not null)
        {
            return _navigateToCurrentToDoItem;
        }

        ConfiguredValueTaskAwaitable<Result> NavigateToCurrentToDoItemAsync(CancellationToken cancellationToken)
        {
            return toDoService
               .GetCurrentActiveToDoItemAsync(cancellationToken)
               .IfSuccessAsync(activeToDoItem =>
                {
                    if (activeToDoItem.IsHasValue)
                    {
                        return navigator.NavigateToAsync<ToDoItemViewModel>(
                            viewModel => viewModel.Id = activeToDoItem.Value.ThrowIfNullStruct().Id, cancellationToken);
                    }

                    return navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
                }, cancellationToken);
        }

        _navigateToCurrentToDoItem = Create(NavigateToCurrentToDoItemAsync, errorHandler, taskProgressService);

        return _navigateToCurrentToDoItem;
    }

    public static SpravyCommand CreateGeneratePassword(
        IPasswordItem passwordItem,
        IPasswordService passwordService,
        IClipboardService clipboard,
        ISpravyNotificationManager spravyNotificationManager,
        IErrorHandler errorHandler, 
        ITaskProgressService taskProgressService
    )
    {
        if (generatePasswordCache.TryGetValue(passwordItem.Id, out var value))
        {
            return value;
        }

        ConfiguredValueTaskAwaitable<Result> GeneratePasswordAsync(CancellationToken cancellationToken)
        {
            return passwordService.GeneratePasswordAsync(passwordItem.Id, cancellationToken)
               .IfSuccessAsync(clipboard.SetTextAsync, cancellationToken)
               .IfSuccessAsync(
                    () => spravyNotificationManager.ShowAsync(
                        new TextLocalization("PasswordGeneratorView.Notification.CopyPassword", passwordItem),
                        cancellationToken), cancellationToken);
        }

        var result = Create(GeneratePasswordAsync, errorHandler, taskProgressService);
        generatePasswordCache.Add(passwordItem.Id, result);

        return result;
    }

    public static SpravyCommand CreateDeletePasswordItem(
        Guid id,
        IPasswordService passwordService,
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler, 
        ITaskProgressService taskProgressService
    )
    {
        if (deletePasswordItemCache.TryGetValue(id, out var value))
        {
            return value;
        }

        ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(CancellationToken cancellationToken)
        {
            return dialogViewer.ShowConfirmContentDialogAsync<DeletePasswordItemViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => passwordService.DeletePasswordItemAsync(id, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.PasswordItemId = id, cancellationToken);
        }

        var result = Create(DeletePasswordItemAsync, errorHandler, taskProgressService);
        deletePasswordItemCache.Add(id, result);

        return result;
    }

    public static SpravyCommand CreateBack(INavigator navigator, IErrorHandler errorHandler, ITaskProgressService taskProgressService)
    {
        if (_back is not null)
        {
            return _back;
        }

        async ValueTask<Result> BackCore(CancellationToken cancellationToken)
        {
            var result = await navigator.NavigateBackAsync(cancellationToken);

            return new(result.Errors);
        }

        _back = Create(c => BackCore(c).ConfigureAwait(false), errorHandler, taskProgressService);

        return _back;
    }

    public static SpravyCommand CreateSendNewVerificationCode(
        IAuthenticationService authenticationService,
        IErrorHandler errorHandler, 
        ITaskProgressService taskProgressService
    )
    {
        if (_sendNewVerificationCode is not null)
        {
            return _sendNewVerificationCode;
        }

        ConfiguredValueTaskAwaitable<Result> SendNewVerificationCodeAsync(
            IVerificationEmail verificationEmail,
            CancellationToken cancellationToken
        )
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
        }

        _sendNewVerificationCode = Create<IVerificationEmail>(SendNewVerificationCodeAsync, errorHandler, taskProgressService);

        return _sendNewVerificationCode;
    }

    public static SpravyCommand CreateNavigateTo<TViewModel>(INavigator navigator, IErrorHandler errorHandler, ITaskProgressService taskProgressService)
        where TViewModel : INavigatable
    {
        if (createNavigateToCache.TryGetValue(typeof(TViewModel), out var command))
        {
            return command;
        }

        var result = Create(navigator.NavigateToAsync<TViewModel>, errorHandler, taskProgressService);
        createNavigateToCache.Add(typeof(TViewModel), result);

        return result;
    }

    public static SpravyCommand Create(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }

        var work = TaskWork.Create(errorHandler, ct => taskProgressService.RunProgressAsync(func, ct));
        var command = ReactiveCommand.CreateFromTask(() => work.RunAsync());
        command.ThrownExceptions.Subscribe(OnNextError);

        return new(work, command);
    }

    public static SpravyCommand Create<TParam>(
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }

        var work = TaskWork.Create<TParam>(errorHandler, (param, ct) => taskProgressService.RunProgressAsync(func, param, ct));
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync<TParam>);
        command.ThrownExceptions.Subscribe(OnNextError);

        return new(work, command);
    }
}