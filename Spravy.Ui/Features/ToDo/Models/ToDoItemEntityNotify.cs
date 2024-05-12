namespace Spravy.Ui.Features.ToDo.Models;

public class ToDoItemEntityNotify : NotifyBase, IEquatable<ToDoItemEntityNotify>
{
    public ToDoItemEntityNotify(
        Guid id,
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
        Path = [RootItem.Default, this,];
        Id = id;
        Description = "Loading...";
        Name = "Loading...";
        Link = string.Empty;
        Status = ToDoItemStatus.ReadyForComplete;
        this.WhenAnyValue(x => x.ReferenceId).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentId)));
        Commands = new();
        AllCommands = new();
        
        CompleteItem = new(MaterialIconKind.Check, new("Command.Complete"), SpravyCommand.Create(cancellationToken =>
        {
            return Result.AwaitableFalse
               .IfSuccessAsync(() =>
                {
                    switch (IsCan)
                    {
                        case ToDoItemIsCan.None:
                            return Result.AwaitableFalse;
                        case ToDoItemIsCan.CanComplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(CurrentId, true, cancellationToken);
                        case ToDoItemIsCan.CanIncomplete:
                            return toDoService.UpdateToDoItemCompleteStatusAsync(CurrentId, false, cancellationToken);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }, cancellationToken)
               .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                    cancellationToken);
        }, errorHandler));
        
        this.WhenAnyValue(x => x.DescriptionType)
           .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDescriptionPlainText));
                this.RaisePropertyChanged(nameof(IsDescriptionMarkdownText));
            });
        
        AddChildItem = new(MaterialIconKind.Plus, new("Command.AddChildToDoItem"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(viewModel => converter
                   .Convert<Uri?>(viewModel.ToDoItemContent.Link)
                   .IfSuccessAsync(uri =>
                    {
                        var options = new AddToDoItemOptions(viewModel.ParentId, viewModel.ToDoItemContent.Name,
                            viewModel.ToDoItemContent.Type, viewModel.DescriptionContent.Description,
                            viewModel.DescriptionContent.Type, uri);
                        
                        return dialogViewer.CloseContentDialogAsync(cancellationToken)
                           .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken),
                                cancellationToken)
                           .IfSuccessAsync(_ => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                                cancellationToken);
                    }, cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                vm => vm.ParentId = CurrentId, cancellationToken), errorHandler));
        
        DeleteItem = new(MaterialIconKind.Delete, new("Command.Delete"), SpravyCommand.Create(cancellationToken =>
            dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(_ => dialogViewer
                   .CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(Id, cancellationToken), cancellationToken)
                   .IfSuccessAsync(() =>
                    {
                        if (ParentId is null)
                        {
                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken);
                        }
                        
                        return navigator.NavigateToAsync<ToDoItemViewModel>(viewModel => viewModel.Id = ParentId.Value,
                            cancellationToken);
                    }, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.ToDoItemId = Id, cancellationToken), errorHandler));
        
        ShowSettingItem = new(MaterialIconKind.Settings, new("Command.Setting"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
                    vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemNameAsync(Id, vm.ToDoItemContent.Name, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(
                            () => converter.Convert<Uri?>(vm.ToDoItemContent.Link)
                               .IfSuccessAsync(uri => toDoService.UpdateToDoItemLinkAsync(Id, uri, cancellationToken),
                                    cancellationToken), cancellationToken)
                       .IfSuccessAsync(
                            () => toDoService.UpdateToDoItemTypeAsync(Id, vm.ToDoItemContent.Type, cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => vm.Settings.ThrowIfNull().ApplySettingsAsync(cancellationToken),
                            cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    vm => vm.ToDoItemId = Id, cancellationToken), errorHandler));
        
        OpenLeafItem = new(MaterialIconKind.Leaf, new("Command.OpenLeaf"),
            SpravyCommand.Create(
                cancellationToken =>
                    navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Id = Id, cancellationToken),
                errorHandler));
        
        ChangeParentItem = new(MaterialIconKind.SwapHorizontal, new("Command.ChangeParent"), SpravyCommand.Create(
            cancellationToken => dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                item => dialogViewer.CloseInputDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.UpdateToDoItemParentAsync(Id, item.Id, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), viewModel =>
                {
                    viewModel.IgnoreIds.Add(Id);
                    viewModel.DefaultSelectedItemId = ParentId.GetValueOrDefault();
                }, cancellationToken), errorHandler));
        
        MakeAsRootItem = new(MaterialIconKind.FamilyTree, new("Command.MakeAsRootToDoItem"),
            SpravyCommand.Create(
                cancellationToken => toDoService.ToDoItemToRootAsync(Id, cancellationToken)
                   .IfSuccessAsync(
                        () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken),
                        cancellationToken), errorHandler));
        
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
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken), errorHandler));
        
        RandomizeChildrenOrderItem = new(MaterialIconKind.DiceSix, new("Command.RandomizeChildrenOrder"),
            SpravyCommand.Create(cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.RandomizeChildrenOrderIndexAsync(CurrentId, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
                {
                    viewModel.Text = "Are you sure?";
                    viewModel.IsReadOnly = true;
                }, cancellationToken), errorHandler));
        
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
                cancellationToken), errorHandler));
        
        ResetItem = new(MaterialIconKind.Refresh, new("Command.Reset"),
            SpravyCommand.Create(
                cancellationToken => dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
                    vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
                       .IfSuccessAsync(
                            () => converter.Convert<ResetToDoItemOptions>(vm)
                               .IfSuccessAsync(o => toDoService.ResetToDoItemAsync(o, cancellationToken),
                                    cancellationToken), cancellationToken)
                       .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                            cancellationToken), _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                    vm => vm.Id = CurrentId, cancellationToken), errorHandler));
        
        AddToFavoriteItem = new(MaterialIconKind.StarOutline, new("Command.AddToFavorite"),
            SpravyCommand.Create(
                cancellationToken => toDoService.AddFavoriteToDoItemAsync(Id, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), errorHandler));
        
        RemoveFromFavoriteItem = new(MaterialIconKind.Star, new("Command.RemoveFromFavorite"),
            SpravyCommand.Create(
                cancellationToken => toDoService.RemoveFavoriteToDoItemAsync(Id, cancellationToken)
                   .IfSuccessAsync(() => uiApplicationService.RefreshCurrentViewAsync(cancellationToken),
                        cancellationToken), errorHandler));
        
        OpenLinkItem = new(MaterialIconKind.Link, new("Command.OpenLink"), SpravyCommand.Create(cancellationToken =>
        {
            var link = Link.ThrowIfNull().ToUri();
            
            return openerLink.OpenLinkAsync(link, cancellationToken);
        }, errorHandler));
        
        NavigateToCurrentItem =
            SpravyCommand.Create(
                cancellationToken => navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = id, cancellationToken),
                errorHandler);
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
    public SpravyCommand NavigateToCurrentItem { get; }
    public AvaloniaList<SpravyCommandNotify> Commands { get; }
    public AvaloniaList<SpravyCommandNotify> AllCommands { get; }
    
    [Reactive]
    public object[] Path { get; set; }
    
    [Reactive]
    public ActiveToDoItemNotify? Active { get; set; }
    
    [Reactive]
    public bool IsSelected { get; set; }
    
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
    public Guid? ParentId { get; set; }
    
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
    
    public ConfiguredValueTaskAwaitable<Result> UpdateCommandsAsync()
    {
        return this.InvokeUIBackgroundAsync(() =>
        {
            Commands.Clear();
            AllCommands.Clear();
            Commands.Add(AddChildItem);
            Commands.Add(ShowSettingItem);
            Commands.Add(DeleteItem);
            Commands.Add(OpenLeafItem);
            Commands.Add(ChangeParentItem);
            Commands.Add(MakeAsRootItem);
            Commands.Add(CopyToClipboardItem);
            Commands.Add(RandomizeChildrenOrderItem);
            Commands.Add(ChangeOrderItem);
            Commands.Add(ResetItem);
            
            if (IsCan.HasFlag(ToDoItemIsCan.CanComplete))
            {
                AllCommands.Add(CompleteItem);
            }
            
            AllCommands.Add(IsFavorite ? RemoveFromFavoriteItem : AddToFavoriteItem);
            
            if (!Link.IsNullOrWhiteSpace())
            {
                AllCommands.Add(OpenLinkItem);
            }
            
            AllCommands.AddRange(Commands);
        });
    }
}