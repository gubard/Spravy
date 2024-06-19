using Spravy.Core.Mappers;
using Spravy.Ui.Mappers;

namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemEntityNotify : NotifyBase, IEquatable<ToDoItemEntityNotify>
{
    public ToDoItemEntityNotify(
        Guid id,
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        Path = [RootItem.Default, this,];
        Id = id;
        Description = "Loading...";
        Name = "Loading...";
        Link = string.Empty;
        Status = ToDoItemStatus.ReadyForComplete;
        OrderIndex = uint.MaxValue;
        this.WhenAnyValue(x => x.ReferenceId).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentId)));
        CompactCommands = new();
        SingleCommands = new();
        Children = new();
        MultiCommands = new();

        CompleteItem = new(MaterialIconKind.Check, new("Command.Complete"), SpravyCommand.Create(cancellationToken =>
        {
            return Result.AwaitableSuccess
               .IfSuccessAsync(() =>
                {
                    switch (IsCan)
                    {
                        case ToDoItemIsCan.None:
                            return Result.AwaitableSuccess;
                        case ToDoItemIsCan.CanComplete:
                            return this.InvokeUiBackgroundAsync(() =>
                                {
                                    IsCan = ToDoItemIsCan.None;
                                    Status = ToDoItemStatus.Completed;

                                    return uiApplicationService.GetCurrentView<IToDoItemUpdater>()
                                       .IfSuccess(u => u.UpdateInListToDoItemUi(this));
                                })
                               .IfErrorsAsync(_ => Result.Success, cancellationToken)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(CurrentId, true,
                                        cancellationToken),
                                    cancellationToken);
                        case ToDoItemIsCan.CanIncomplete:
                            return this.InvokeUiBackgroundAsync(() =>
                                {
                                    IsCan = ToDoItemIsCan.None;
                                    Status = ToDoItemStatus.ReadyForComplete;

                                    return uiApplicationService.GetCurrentView<IToDoItemUpdater>()
                                       .IfSuccess(u => u.UpdateInListToDoItemUi(this));
                                })
                               .IfErrorsAsync(_ => Result.Success, cancellationToken)
                               .IfSuccessAsync(
                                    () => toDoService.UpdateToDoItemCompleteStatusAsync(CurrentId, false,
                                        cancellationToken),
                                    cancellationToken);
                        default:
                            return new Result(new ToDoItemIsCanOutOfRangeError(IsCan)).ToValueTaskResult()
                               .ConfigureAwait(false);
                    }
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler, taskProgressService));

        this.WhenAnyValue(x => x.DescriptionType)
           .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
            });

        AddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                    viewModel => viewModel.ConverterToAddToDoItemOptions()
                       .IfSuccessAsync(
                            options => dialogViewer.CloseContentDialogAsync(cancellationToken)
                               .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                    cancellationToken)
                               .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                    cancellationToken), cancellationToken),
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ParentId = CurrentId,
                    cancellationToken), errorHandler, taskProgressService));

        DeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"), SpravyCommand.Create(cancellationToken =>
            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(_ => dialogViewer
                   .CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(Id, cancellationToken), cancellationToken)
                   .IfSuccessAsync(uiApplicationService.GetCurrentViewType, cancellationToken)
                   .IfSuccessAsync(type =>
                    {
                        if (type != typeof(ToDoItemViewModel))
                        {
                            return Result.AwaitableSuccess;
                        }

                        if (Parent is null)
                        {
                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken);
                        }

                        return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = Parent.Id,
                            cancellationToken);
                    }, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Item = this, cancellationToken), errorHandler, taskProgressService));

        ShowSettingItem = new(MaterialIconKind.Settings, new("Command.Setting"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(vm =>
                    dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemNameAsync(Id, vm.ToDoItemContent.Name, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() =>
                        {
                            if (vm.ToDoItemContent.Link.IsNullOrWhiteSpace())
                            {
                                return toDoService.UpdateToDoItemLinkAsync(Id, new(null), cancellationToken);
                            }

                            return toDoService.UpdateToDoItemLinkAsync(Id, vm.ToDoItemContent.Link.ToOptionUri(),
                                cancellationToken);
                        }, cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemTypeAsync(Id, vm.ToDoItemContent.Type, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => vm.Settings.ThrowIfNull().ApplySettingsAsync(cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                vm => vm.ToDoItemId = Id, cancellationToken), errorHandler, taskProgressService));

        OpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"),
            SpravyCommand.Create(
                cancellationToken =>
                    navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Item = this, cancellationToken),
                errorHandler, taskProgressService));

        ChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                item => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.UpdateToDoItemParentAsync(Id, item.Id, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), viewModel =>
                {
                    viewModel.IgnoreIds = new([id,]);
                    viewModel.DefaultSelectedItemId = (Parent?.Id).GetValueOrDefault();
                }, cancellationToken), errorHandler, taskProgressService));

        MakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"),
            SpravyCommand.Create(
                cancellationToken => toDoService.ToDoItemToRootAsync(Id, cancellationToken)
                   .IfSuccessAsync(
                        () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken),
                        cancellationToken), errorHandler, taskProgressService));

        CopyToClipboardItem = new(MaterialIconKind.Clipboard, new("Command.CopyToClipboard"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowConfirmContentDialogAsync(view =>
                {
                    var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                    var options = new ToDoItemToStringOptions(statuses, CurrentId);

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.ToDoItemToStringAsync(options, cancellationToken)
                               .IfSuccessAsync(clipboardService.SetTextAsync, cancellationToken), cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken), errorHandler, taskProgressService));

        RandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<RandomizeChildrenOrderViewModel>(
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => toDoService.RandomizeChildrenOrderIndexAsync(CurrentId, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    viewModel => viewModel.Item = this, cancellationToken), errorHandler, taskProgressService));

        ChangeOrderItem = new(MaterialIconKind.ReorderHorizontal, new("Command.Reorder"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                viewModel =>
                {
                    var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                    var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);

                    return dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(() => toDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken);
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel => viewModel.Id = Id,
                cancellationToken), errorHandler, taskProgressService));

        ResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                    vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.ResetToDoItemAsync(vm.ToResetToDoItemOptions(), cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    vm => vm.Id = CurrentId, cancellationToken), errorHandler, taskProgressService));

        AddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"),
            SpravyCommand.Create(
                cancellationToken => toDoService.AddFavoriteToDoItemAsync(Id, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), errorHandler, taskProgressService));

        RemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            SpravyCommand.Create(
                cancellationToken => toDoService.RemoveFavoriteToDoItemAsync(Id, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), errorHandler, taskProgressService));

        OpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"), SpravyCommand.Create(cancellationToken =>
        {
            var link = Link.ThrowIfNull().ToUri();

            return openerLink.OpenLinkAsync(link, cancellationToken);
        }, errorHandler, taskProgressService));

        NavigateToCurrentItem =
            SpravyCommand.Create(
                cancellationToken =>
                    navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = CurrentId, cancellationToken),
                errorHandler, taskProgressService);

        CloneItem = new(MaterialIconKind.Copyleft, new("Command.Clone"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                    itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.CloneToDoItemAsync(Id, itemNotify.Id.ToOption(),
                                cancellationToken), cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), view => view.DefaultSelectedItemId = Id, cancellationToken),
                errorHandler, taskProgressService));

        MultiAddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

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
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ParentId = CurrentId,
                    cancellationToken);
            }, errorHandler, taskProgressService));

        MultiShowSettingItem = new(MaterialIconKind.Settings, new("Command.Setting"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

                if (selected.IsEmpty)
                {
                    return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                }

                return dialogViewer.ShowConfirmContentDialogAsync<MultiToDoItemSettingViewModel>(vm => dialogViewer
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
                    vm => vm.ToDoItemId = Id, cancellationToken);
            }, errorHandler, taskProgressService));

        MultiDeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"), SpravyCommand.Create(cancellationToken =>
        {
            ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

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
                    vm.Item = this;
                    vm.DeleteItems.Update(selected);
                }, cancellationToken);
        }, errorHandler, taskProgressService));

        MultiOpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                if (selected.IsEmpty)
                {
                    return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                }

                return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm =>
                {
                    vm.Item = this;
                    vm.LeafIds = selected;
                }, cancellationToken);
            }, errorHandler, taskProgressService));

        MultiChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

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
                        viewModel.DefaultSelectedItemId = Id;
                    }, cancellationToken);
            }, errorHandler, taskProgressService));

        MultiMakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                if (selected.IsEmpty)
                {
                    return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                }

                return selected.ToResult()
                   .IfSuccessForEachAsync(i => toDoService.ToDoItemToRootAsync(i, cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken);
            }, errorHandler, taskProgressService));

        MultiCopyToClipboardItem = new(MaterialIconKind.Clipboard, new("Command.CopyToClipboard"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

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
            }, errorHandler, taskProgressService));

        MultiRandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            SpravyCommand.Create(cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

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
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    viewModel =>
                    {
                        viewModel.Item = this;
                        viewModel.RandomizeChildrenOrderIds = selected;
                    }, cancellationToken);
            }, errorHandler, taskProgressService));

        MultiChangeOrderItem = new(MaterialIconKind.ReorderHorizontal, new("Command.Reorder"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).Reverse().ToArray();

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
                                        new(i, selectedItem.Id, viewModel.IsAfter), cancellationToken),
                                    cancellationToken)
                               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                    cancellationToken), cancellationToken),
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                    {
                        viewModel.Id = Id;

                        viewModel.ChangeToDoItemOrderIndexIds =
                            Children.Where(x => !x.IsSelected).Select(x => x.Id).ToArray();
                    }, cancellationToken);
            }, errorHandler, taskProgressService));

        MultiResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"), SpravyCommand.Create(cancellationToken =>
            dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(vm =>
                {
                    ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

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
                }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.Id = CurrentId,
                cancellationToken), errorHandler, taskProgressService));

        MultiCloneItem = new(MaterialIconKind.Copyleft, new("Command.Clone"), SpravyCommand.Create(cancellationToken =>
        {
            ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

            if (selected.IsEmpty)
            {
                return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
            }

            return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => selected.ToResult(), cancellationToken)
                   .IfSuccessForEachAsync(
                        i => toDoService.CloneToDoItemAsync(i, itemNotify.Id.ToOption(),
                            cancellationToken), cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), view => view.DefaultSelectedItemId = Id, cancellationToken);
        }, errorHandler, taskProgressService));

        MultiOpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

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
            }, errorHandler, taskProgressService));

        MultiAddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                if (selected.IsEmpty)
                {
                    return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                }

                return selected.ToResult()
                   .IfSuccessForEachAsync(i => toDoService.AddFavoriteToDoItemAsync(i, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken);
            }, errorHandler, taskProgressService));

        MultiRemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            SpravyCommand.Create(cancellationToken =>
            {
                ReadOnlyMemory<Guid> selected = Children.Where(x => x.IsSelected).Select(x => x.Id).ToArray();

                if (selected.IsEmpty)
                {
                    return new Result(new NonItemSelectedError()).ToValueTaskResult().ConfigureAwait(false);
                }

                return selected.ToResult()
                   .IfSuccessForEachAsync(i => toDoService.RemoveFavoriteToDoItemAsync(i, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken);
            }, errorHandler, taskProgressService));

        MultiCompleteItem = new(MaterialIconKind.Check, new("Command.Complete"), SpravyCommand.Create(
            cancellationToken =>
            {
                ReadOnlyMemory<ToDoItemEntityNotify> selected = Children.Where(x => x.IsSelected).ToArray();

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
            }, errorHandler, taskProgressService));

        MultiCommands.AddRange([
            MultiAddChildItem,
            MultiShowSettingItem,
            MultiDeleteItem,
            MultiOpenLeafItem,
            MultiChangeParentItem,
            MultiMakeAsRootItem,
            MultiCopyToClipboardItem,
            MultiRandomizeChildrenOrderItem,
            MultiChangeOrderItem,
            MultiResetItem,
            MultiCloneItem,
            MultiOpenLinkItem,
            MultiAddToFavoriteItem,
            MultiRemoveFromFavoriteItem,
            MultiCompleteItem,
        ]);
    }

    public Guid Id { get; }
    public SpravyCommandNotify CompleteItem { get; }
    public SpravyCommandNotify AddToFavoriteItem { get; }
    public SpravyCommandNotify RemoveFromFavoriteItem { get; }
    public SpravyCommandNotify OpenLinkItem { get; }
    public SpravyCommandNotify AddChildItem { get; }
    public SpravyCommandNotify DeleteItem { get; }
    public SpravyCommandNotify ShowSettingItem { get; }
    public SpravyCommandNotify OpenLeafItem { get; }
    public SpravyCommandNotify ChangeParentItem { get; }
    public SpravyCommandNotify MakeAsRootItem { get; }
    public SpravyCommandNotify CopyToClipboardItem { get; }
    public SpravyCommandNotify RandomizeChildrenOrderItem { get; }
    public SpravyCommandNotify ChangeOrderItem { get; }
    public SpravyCommandNotify ResetItem { get; }
    public SpravyCommandNotify CloneItem { get; }

    public SpravyCommandNotify MultiCompleteItem { get; }
    public SpravyCommandNotify MultiAddToFavoriteItem { get; }
    public SpravyCommandNotify MultiRemoveFromFavoriteItem { get; }
    public SpravyCommandNotify MultiOpenLinkItem { get; }
    public SpravyCommandNotify MultiAddChildItem { get; }
    public SpravyCommandNotify MultiDeleteItem { get; }
    public SpravyCommandNotify MultiShowSettingItem { get; }
    public SpravyCommandNotify MultiOpenLeafItem { get; }
    public SpravyCommandNotify MultiChangeParentItem { get; }
    public SpravyCommandNotify MultiMakeAsRootItem { get; }
    public SpravyCommandNotify MultiCopyToClipboardItem { get; }
    public SpravyCommandNotify MultiRandomizeChildrenOrderItem { get; }
    public SpravyCommandNotify MultiChangeOrderItem { get; }
    public SpravyCommandNotify MultiResetItem { get; }
    public SpravyCommandNotify MultiCloneItem { get; }

    public SpravyCommand NavigateToCurrentItem { get; }
    public AvaloniaList<SpravyCommandNotify> CompactCommands { get; }
    public AvaloniaList<SpravyCommandNotify> SingleCommands { get; }
    public AvaloniaList<SpravyCommandNotify> MultiCommands { get; }
    public AvaloniaList<ToDoItemEntityNotify> Children { get; }

    [Reactive]
    public object[] Path { get; set; }

    [Reactive]
    public ActiveToDoItemNotify? Active { get; set; }

    [Reactive]
    public bool IsSelected { get; set; }

    [Reactive]
    public bool IsExpanded { get; set; }

    [Reactive]
    public bool IsIgnore { get; set; }

    [Reactive]
    public bool IsFavorite { get; set; }

    [Reactive]
    public string Description { get; set; }

    [Reactive]
    public uint OrderIndex { get; set; }

    [Reactive]
    public ToDoItemStatus Status { get; set; }

    [Reactive]
    public ToDoItemIsCan IsCan { get; set; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public ToDoItemEntityNotify? Parent { get; set; }

    [Reactive]
    public Guid? ReferenceId { get; set; }

    [Reactive]
    public DescriptionType DescriptionType { get; set; }

    [Reactive]
    public string Link { get; set; }

    [Reactive]
    public ToDoItemType Type { get; set; }

    public bool IsDescriptionPlainText
    {
        get => DescriptionType == DescriptionType.PlainText;
    }

    public bool IsDescriptionMarkdownText
    {
        get => DescriptionType == DescriptionType.Markdown;
    }

    public Guid CurrentId
    {
        get => ReferenceId ?? Id;
    }

    public bool Equals(ToDoItemEntityNotify? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ToDoItemEntityNotify)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public Result<ToDoItemEntityNotify> UpdateCommandsUi()
    {
        CompactCommands.Clear();
        SingleCommands.Clear();

        CompactCommands.AddRange([
            AddChildItem,
            ShowSettingItem,
            DeleteItem,
            OpenLeafItem,
            ChangeParentItem,
            MakeAsRootItem,
            CopyToClipboardItem,
            RandomizeChildrenOrderItem,
            ChangeOrderItem,
            ResetItem,
            CloneItem,
        ]);

        var singleCommands = new List<SpravyCommandNotify>(CompactCommands);

        if (!Link.IsNullOrWhiteSpace())
        {
            singleCommands.Add(OpenLinkItem);
        }

        singleCommands.Add(IsFavorite ? RemoveFromFavoriteItem : AddToFavoriteItem);

        if (IsCan != ToDoItemIsCan.None)
        {
            singleCommands.Add(CompleteItem);
        }

        SingleCommands.AddRange(singleCommands);

        return this.ToResult();
    }
}