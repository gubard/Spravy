namespace Spravy.Ui.Features.ToDo.Commands;

public class RootToDoItemsCommands
{
    public RootToDoItemsCommands(
        IToDoService toDoService,
        INavigator navigator,
        IUiApplicationService uiApplicationService,
        IDialogViewer dialogViewer,
        IConverter converter,
        IClipboardService clipboardService,
        IOpenerLink openerLink,
        IErrorHandler errorHandler
    )
    {
        MultiAddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
                
                return dialogViewer.ShowConfirmContentDialogAsync(viewModel => converter
                       .Convert<Uri?>(viewModel.ToDoItemContent.Link)
                       .IfSuccessAsync(uri => selected.ToResult()
                           .IfSuccessForEachAsync(item =>
                            {
                                var options = new AddToDoItemOptions(item.Id, viewModel.ToDoItemContent.Name,
                                    viewModel.ToDoItemContent.Type, viewModel.DescriptionContent.Description,
                                    viewModel.DescriptionContent.Type, uri);
                                
                                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                                   .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                        cancellationToken)
                                   .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                        cancellationToken);
                            }, cancellationToken), cancellationToken),
                    _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    ActionHelper<AddToDoItemViewModel>.Empty,
                    cancellationToken);
            }, errorHandler));
        
        MultiShowSettingItem = new(MaterialIconKind.Settings, new("Command.Setting"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
                                        return toDoService.UpdateToDoItemLinkAsync(item.Id,
                                            vm.Link.IsNullOrWhiteSpace() ? null : vm.Link.ToUri(), cancellationToken);
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
            }, errorHandler));
        
        MultiDeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"), SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
            
            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                
                ;
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
                    vm.DeletedIds = selected.ToArray().Select(x => x.Id).ToArray();
                }, cancellationToken);
        }, errorHandler));
        
        MultiOpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiMakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiCopyToClipboardItem = new(MaterialIconKind.Clipboard, new("Command.CopyToClipboard"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiRandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            SpravyCommand.Create(cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    viewModel =>
                    {
                        viewModel.RandomizeChildrenOrderIds = selected;
                    }, cancellationToken);
            }, errorHandler));
        
        MultiChangeOrderItem = new(MaterialIconKind.ReorderHorizontal, new("Command.Reorder"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
                                        new(i, selectedItem.Id, viewModel.IsAfter), cancellationToken),
                                    cancellationToken)
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
            }, errorHandler));
        
        MultiResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"), SpravyCommand.Create(cancellationToken =>
            dialogViewer.ShowConfirmContentDialogAsync(vm =>
                {
                    var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                    
                    if (view.IsHasError)
                    {
                        return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                        
                        ;
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
                cancellationToken), errorHandler));
        
        MultiCloneItem = new(MaterialIconKind.Copyleft, new("Command.Clone"), SpravyCommand.Create(cancellationToken =>
        {
            var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
            
            if (view.IsHasError)
            {
                return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                
                ;
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
                   .IfSuccessForEachAsync(i => toDoService.CloneToDoItemAsync(i, itemNotify.Id, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), ActionHelper<ToDoItemSelectorViewModel>.Empty, cancellationToken);
        }, errorHandler));
        
        MultiOpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiAddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiRemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            SpravyCommand.Create(cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
        
        MultiCompleteItem = new(MaterialIconKind.Check, new("Command.Complete"), SpravyCommand.Create(
            cancellationToken =>
            {
                var view = uiApplicationService.GetCurrentView<RootToDoItemsViewModel>();
                
                if (view.IsHasError)
                {
                    return new Result(view.Errors).ToValueTaskResult().ConfigureAwait(false);
                    
                    ;
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
            }, errorHandler));
    }
    
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
}