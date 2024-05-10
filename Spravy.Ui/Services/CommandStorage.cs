namespace Spravy.Ui.Services;

public static class CommandStorage
{
    private static readonly INavigator navigator;
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IToDoService toDoService;
    private static readonly IOpenerLink openerLink;
    private static readonly IMapper mapper;
    private static readonly IAuthenticationService authenticationService;
    private static readonly IObjectStorage objectStorage;
    private static readonly IClipboardService clipboard;
    private static readonly IPasswordService passwordService;
    private static readonly ISpravyNotificationManager spravyNotificationManager;
    private static readonly IErrorHandler errorHandler;
    
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        clipboard = kernel.Get<IClipboardService>();
        passwordService = kernel.Get<IPasswordService>();
        spravyNotificationManager = kernel.Get<ISpravyNotificationManager>();
        objectStorage = kernel.Get<IObjectStorage>();
        mapper = kernel.Get<IMapper>();
        authenticationService = kernel.Get<IAuthenticationService>();
        navigator = kernel.Get<INavigator>();
        openerLink = kernel.Get<IOpenerLink>();
        dialogViewer = kernel.Get<IDialogViewer>();
        mainSplitViewModel = kernel.Get<MainSplitViewModel>();
        toDoService = kernel.Get<IToDoService>();
        errorHandler = kernel.Get<IErrorHandler>();
        SwitchPaneItem = CreateCommand(SwitchPaneAsync, MaterialIconKind.Menu, "Open pane");
        NavigateToToDoItemItem = CreateCommand<Guid>(NavigateToToDoItemAsync, MaterialIconKind.ListBox, "Open");
        
        SwitchCompleteToDoItemItem = CreateCommand<ICanCompleteProperty>(
            SwitchCompleteToDoItemAsync, MaterialIconKind.Check, "Complete");
        
        SelectAll = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(SelectAllAsync, MaterialIconKind.CheckAll,
            "Select all");
        
        DeleteToDoItemItem = CreateCommand<IDeletable>(DeleteToDoItemAsync, MaterialIconKind.Delete, "Delete");
        
        ChangeOrderIndexItem = CreateCommand<ToDoItemNotify>(
            ChangeOrderIndexAsync, MaterialIconKind.ReorderHorizontal, "Reorder");
        
        OpenLinkItem = CreateCommand<ILink>(OpenLinkAsync, MaterialIconKind.Link, "Open link");
        
        RemoveToDoItemFromFavoriteItem = CreateCommand<Guid>(
            RemoveFavoriteToDoItemAsync, MaterialIconKind.Star, "Remove from favorite");
        
        AddToDoItemToFavoriteItem = CreateCommand<Guid>(
            AddFavoriteToDoItemAsync, MaterialIconKind.StarOutline, "Add to favorite");
        
        SetToDoShortItemItem = CreateCommand<IToDoShortItemProperty>(
            SetToDoShortItemAsync, MaterialIconKind.Pencil, "Set to-do item");
        
        SetDueDateTimeItem = CreateCommand<IDueDateTimeProperty>(
            SetDueDateTimeAsync, MaterialIconKind.Pencil, "Set due date time");
        
        BackItem = CreateCommand(BackAsync, MaterialIconKind.ArrowLeft, "Back");
        NavigateToItem = CreateCommand<Type>(NavigateToAsync, MaterialIconKind.ArrowLeft, "Navigate to");
        LogoutItem = CreateCommand(LogoutAsync, MaterialIconKind.Logout, "Logout");
        AddRootToDoItemItem = CreateCommand(AddRootToDoItemAsync, MaterialIconKind.Plus, "Add root to-do item");
        
        ToDoItemSearchItem = CreateCommand<IToDoItemSearchProperties>(
            ToDoItemSearchAsync, MaterialIconKind.Search, "Search to-do item");
        
        SetToDoDescriptionItem = CreateCommand<IToDoDescriptionProperty>(SetToDoDescriptionAsync,
            MaterialIconKind.Pencil, "Set to-do item description");
        
        ShowToDoSettingItem = CreateCommand<IToDoSettingsProperty>(
            ShowToDoSettingAsync, MaterialIconKind.Settings, "Show to-do setting");
        
        AddToDoItemChildItem = CreateCommand<ICurrentIdProperty>(
            AddToDoItemChildAsync, MaterialIconKind.Plus, "Add child task");
        
        NavigateToLeafItem = CreateCommand<Guid>(NavigateToLeafAsync, MaterialIconKind.Leaf, "Navigate to leaf");
        
        SetToDoParentItemItem = CreateCommand<ISetToDoParentItemParams>(SetToDoParentItemAsync,
            MaterialIconKind.SwapHorizontal, "Set to-do item parent");
        
        MoveToDoItemToRootItem = CreateCommand<IIdProperty>(MoveToDoItemToRootAsync, MaterialIconKind.FamilyTree,
            "Move to-do item to root");
        
        ToDoItemToStringItem = CreateCommand<ICurrentIdProperty>(
            ToDoItemToStringAsync, MaterialIconKind.ContentCopy, "Copy to-do item");
        
        ToDoItemRandomizeChildrenOrderIndexItem = CreateCommand<ICurrentIdProperty>(
            ToDoItemRandomizeChildrenOrderIndexAsync, MaterialIconKind.Dice6Outline, "Randomize children order");
        
        NavigateToCurrentToDoItemItem = CreateCommand(NavigateToCurrentToDoItemAsync, MaterialIconKind.ArrowRight,
            "Open current to-do item");
        
        MultiCompleteToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSwitchCompleteToDoItemsAsync, MaterialIconKind.CheckAll, "Complete all to-do items");
        
        MultiSetParentToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSetParentToDoItemsAsync, MaterialIconKind.SwapHorizontal, "Set parent for all to-do items");
        
        MultiMoveToDoItemsToRootItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiMoveToDoItemsToRootAsync, MaterialIconKind.FamilyTree, "Move items to root");
        
        MultiSetTypeToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(MultiSetTypeToDoItemsAsync,
            MaterialIconKind.Switch, "Set type all to-do items");
        
        SendNewVerificationCodeItem = CreateCommand<IVerificationEmail>(SendNewVerificationCodeAsync,
            MaterialIconKind.CodeString, "Verification email");
        
        ResetToDoItemItem = CreateCommand<ICurrentIdProperty>(
            ResetToDoItemAsync, MaterialIconKind.EncryptionReset, "Reset to-do item");
        
        MultiDeleteToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(MultiDeleteToDoItemsAsync,
            MaterialIconKind.Delete, "Delete to-do items");
        
        CloneToDoItemItem = CreateCommand<IIdProperty>(
            CloneToDoItemAsync, MaterialIconKind.FileMove, "Clone to-do item");
        
        AddPasswordItemItem = CreateCommand(AddPasswordItemAsync, MaterialIconKind.Plus, "Add password item");
        
        ShowPasswordItemSettingItem = CreateCommand<IIdProperty>(ShowPasswordItemSettingAsync,
            MaterialIconKind.Settings, "Show password setting");
    }
    
    public static CommandItem CloneToDoItemItem { get; }
    public static CommandItem ResetToDoItemItem { get; }
    
    public static ICommand SendNewVerificationCodeCommand
    {
        get => SendNewVerificationCodeItem.Command;
    }
    
    public static CommandItem SendNewVerificationCodeItem { get; }
    public static CommandItem MultiSetTypeToDoItemsItem { get; }
    public static CommandItem MultiMoveToDoItemsToRootItem { get; }
    public static CommandItem MultiSetParentToDoItemsItem { get; }
    public static CommandItem MultiCompleteToDoItemsItem { get; }
    public static CommandItem NavigateToCurrentToDoItemItem { get; }
    public static CommandItem ToDoItemRandomizeChildrenOrderIndexItem { get; }
    public static CommandItem ToDoItemToStringItem { get; }
    public static CommandItem MoveToDoItemToRootItem { get; }
    public static CommandItem NavigateToLeafItem { get; }
    public static CommandItem AddToDoItemChildItem { get; }
    public static CommandItem ShowToDoSettingItem { get; }
    
    public static ICommand SetToDoDescriptionCommand
    {
        get => SetToDoDescriptionItem.Command;
    }
    
    public static CommandItem SetToDoDescriptionItem { get; }
    
    public static ICommand ToDoItemSearchCommand
    {
        get => ToDoItemSearchItem.Command;
    }
    
    public static CommandItem ToDoItemSearchItem { get; }
    
    public static ICommand SwitchPaneCommand
    {
        get => SwitchPaneItem.Command;
    }
    
    public static CommandItem SwitchPaneItem { get; }
    public static CommandItem SwitchCompleteToDoItemItem { get; }
    public static CommandItem ChangeOrderIndexItem { get; }
    public static CommandItem DeleteToDoItemItem { get; }
    
    public static ICommand NavigateToToDoItemCommand
    {
        get => NavigateToToDoItemItem.Command;
    }
    
    public static CommandItem NavigateToToDoItemItem { get; }
    
    public static ICommand OpenLinkCommand
    {
        get => OpenLinkItem.Command;
    }
    
    public static CommandItem OpenLinkItem { get; }
    
    public static ICommand RemoveToDoItemFromFavoriteCommand
    {
        get => RemoveToDoItemFromFavoriteItem.Command;
    }
    
    public static CommandItem RemoveToDoItemFromFavoriteItem { get; }
    
    public static ICommand AddToDoItemToFavoriteCommand
    {
        get => AddToDoItemToFavoriteItem.Command;
    }
    
    public static CommandItem AddToDoItemToFavoriteItem { get; }
    
    public static ICommand SetToDoShortItemCommand
    {
        get => SetToDoShortItemItem.Command;
    }
    
    public static CommandItem SetToDoShortItemItem { get; }
    
    public static ICommand SetDueDateTimeCommand
    {
        get => SetDueDateTimeItem.Command;
    }
    
    public static CommandItem SetDueDateTimeItem { get; }
    
    public static ICommand BackCommand
    {
        get => BackItem.Command;
    }
    
    public static CommandItem BackItem { get; }
    
    public static ICommand NavigateToCommand
    {
        get => NavigateToItem.Command;
    }
    
    public static CommandItem NavigateToItem { get; }
    
    public static ICommand LogoutCommand
    {
        get => LogoutItem.Command;
    }
    
    public static CommandItem LogoutItem { get; }
    
    public static ICommand AddRootToDoItemCommand
    {
        get => AddRootToDoItemItem.Command;
    }
    
    public static CommandItem AddRootToDoItemItem { get; }
    public static CommandItem SetToDoParentItemItem { get; }
    public static CommandItem MultiDeleteToDoItemsItem { get; }
    
    public static ICommand AddPasswordItemCommand
    {
        get => AddPasswordItemItem.Command;
    }
    
    public static CommandItem AddPasswordItemItem { get; }
    
    public static ICommand ShowPasswordItemSettingCommand
    {
        get => ShowPasswordItemSettingItem.Command;
    }
    
    public static CommandItem ShowPasswordItemSettingItem { get; }
    
    public static CommandItem SelectAll { get; }
    
    private static ConfiguredValueTaskAwaitable<Result> ShowPasswordItemSettingAsync(
        IIdProperty idProperty,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<PasswordItemSettingsViewModel>(
            vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemKeyAsync(idProperty.Id, vm.Key, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemLengthAsync(idProperty.Id, vm.Length, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemNameAsync(idProperty.Id, vm.Name, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemRegexAsync(idProperty.Id, vm.Regex, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemCustomAvailableCharactersAsync(idProperty.Id,
                        vm.CustomAvailableCharacters, cancellationToken), cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemIsAvailableNumberAsync(idProperty.Id, vm.IsAvailableNumber,
                        cancellationToken), cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemIsAvailableLowerLatinAsync(idProperty.Id,
                        vm.IsAvailableLowerLatin, cancellationToken), cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(idProperty.Id,
                        vm.IsAvailableSpecialSymbols, cancellationToken), cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.UpdatePasswordItemIsAvailableUpperLatinAsync(idProperty.Id,
                        vm.IsAvailableUpperLatin, cancellationToken), cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.Id = idProperty.Id,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => passwordService.AddPasswordItemAsync(mapper.Map<AddPasswordOptions>(vm), cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<AddPasswordItemViewModel>.Empty,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiDeleteToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> items,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAllAsync(cancellationToken, items.Where(x => x.IsSelect)
               .Select(x => x.Value.Id)
               .Select<Guid, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                {
                    var y = x;
                    
                    return () => toDoService.DeleteToDoItemAsync(y, cancellationToken);
                })
               .ToArray())
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(
        ICurrentIdProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<ResetToDoItemViewModel>(
            vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => toDoService.ResetToDoItemAsync(mapper.Map<ResetToDoItemOptions>(vm), cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.Id = property.CurrentId,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SendNewVerificationCodeAsync(
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
            default: throw new ArgumentOutOfRangeException();
        }
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiMoveToDoItemsToRootAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAllAsync(cancellationToken, itemsNotify.Where(x => x.IsSelect)
               .Select<Selected<ToDoItemNotify>, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                {
                    var id = x.Value.Id;
                    
                    return () => toDoService.ToDoItemToRootAsync(id, cancellationToken);
                })
               .ToArray())
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiSetTypeToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();
        
        return dialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(type => dialogViewer
           .CloseInputDialogAsync(cancellationToken)
           .IfSuccessAllAsync(cancellationToken, ids.Select<Guid, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                {
                    var y = x;
                    
                    return () => toDoService.UpdateToDoItemTypeAsync(y, type, cancellationToken);
                })
               .ToArray())
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken), viewModel =>
        {
            viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
            viewModel.SelectedItem = viewModel.Items.First();
        }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiSetParentToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();
        
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(item => dialogViewer
               .CloseInputDialogAsync(cancellationToken)
               .IfSuccessAllAsync(cancellationToken, ids.Select<Guid, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                    {
                        var y = x;
                        
                        return () => toDoService.UpdateToDoItemParentAsync(y, item.Id, cancellationToken);
                    })
                   .ToArray())
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            viewModel => viewModel.IgnoreIds.AddRange(ids), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiSwitchCompleteToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var items = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value).ToArray();
        
        return CompleteAsync(items, cancellationToken)
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> CompleteAsync(
        IEnumerable<ToDoItemNotify> items,
        CancellationToken cancellationToken
    )
    {
        var tasks = new List<Func<ConfiguredValueTaskAwaitable<Result>>>();
        
        foreach (var item in items)
        {
            switch (item.IsCan)
            {
                case ToDoItemIsCan.None:
                    break;
                case ToDoItemIsCan.CanComplete:
                    tasks.Add(() => toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken));
                    
                    break;
                case ToDoItemIsCan.CanIncomplete:
                    tasks.Add(() => toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, false, cancellationToken));
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        return Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken, tasks.ToArray());
    }
    
    private static ConfiguredValueTaskAwaitable<Result> NavigateToCurrentToDoItemAsync(
        CancellationToken cancellationToken
    )
    {
        return toDoService.GetCurrentActiveToDoItemAsync(cancellationToken)
           .IfSuccessAsync(activeToDoItem =>
            {
                if (activeToDoItem.HasValue)
                {
                    return navigator.NavigateToAsync<ToDoItemViewModel>(
                        viewModel => viewModel.Id = activeToDoItem.Value.Id,
                        cancellationToken);
                }
                
                return navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
            }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ToDoItemRandomizeChildrenOrderIndexAsync(
        ICurrentIdProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => toDoService.RandomizeChildrenOrderIndexAsync(property.CurrentId, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
            {
                viewModel.Text = "Are you sure?";
                viewModel.IsReadOnly = true;
            }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ToDoItemToStringAsync(
        ICurrentIdProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync(view =>
            {
                var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                var options = new ToDoItemToStringOptions(statuses, property.CurrentId);
                
                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(
                        () => toDoService.ToDoItemToStringAsync(options, cancellationToken)
                           .IfSuccessAsync(text => clipboard.SetTextAsync(text), cancellationToken), cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<ToDoItemToStringSettingsViewModel>.Empty, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MoveToDoItemToRootAsync(
        IIdProperty property,
        CancellationToken cancellationToken
    )
    {
        return toDoService.ToDoItemToRootAsync(property.Id, cancellationToken)
           .IfSuccessAsync(
                () => navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken),
                cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SetToDoParentItemAsync(
        ISetToDoParentItemParams property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            item => dialogViewer.CloseInputDialogAsync(cancellationToken)
               .IfSuccessAsync(() => toDoService.UpdateToDoItemParentAsync(property.Id, item.Id, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken), viewModel =>
            {
                viewModel.IgnoreIds.Add(property.Id);
                viewModel.DefaultSelectedItemId = property.ParentId.GetValueOrDefault();
            }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> NavigateToLeafAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Id = id, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> AddToDoItemChildAsync(
        ICurrentIdProperty item,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(viewModel =>
            {
                var options = new AddToDoItemOptions(viewModel.ParentId, viewModel.ToDoItemContent.Name,
                    viewModel.ToDoItemContent.Type, viewModel.DescriptionContent.Description,
                    viewModel.DescriptionContent.Type, mapper.Map<Uri?>(viewModel.ToDoItemContent.Link));
                
                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.AddToDoItemAsync(options, cancellationToken), cancellationToken)
                   .IfSuccessAsync(_ => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ParentId = item.CurrentId,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ShowToDoSettingAsync(
        IToDoSettingsProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
            vm => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAllAsync(cancellationToken,
                    () => toDoService.UpdateToDoItemNameAsync(property.Id, vm.ToDoItemContent.Name, cancellationToken),
                    () => toDoService.UpdateToDoItemLinkAsync(property.Id, mapper.Map<Uri?>(vm.ToDoItemContent.Link),
                        cancellationToken),
                    () => toDoService.UpdateToDoItemTypeAsync(property.Id, vm.ToDoItemContent.Type, cancellationToken),
                    () => vm.Settings.ThrowIfNull().ApplySettingsAsync(cancellationToken))
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), vm => vm.ToDoItemId = property.Id,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SetToDoDescriptionAsync(
        IToDoDescriptionProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
            viewModel => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => toDoService.UpdateToDoItemDescriptionAsync(property.Id, viewModel.Content.Description,
                        cancellationToken), cancellationToken)
               .IfSuccessAsync(() => property.RefreshAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel =>
            {
                viewModel.Content.Description = property.Description;
                viewModel.Content.Type = property.DescriptionType;
                viewModel.ToDoItemName = property.Name;
            }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ToDoItemSearchAsync(
        IToDoItemSearchProperties properties,
        CancellationToken cancellationToken
    )
    {
        return toDoService.SearchToDoItemIdsAsync(properties.SearchText, cancellationToken)
           .IfSuccessAsync(
                ids => properties.ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), properties, false,
                    cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> AddRootToDoItemAsync(CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(view =>
            {
                var options = mapper.Map<AddRootToDoItemOptions>(view);
                
                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.AddRootToDoItemAsync(options, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(_ => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<AddRootToDoItemViewModel>.Empty,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> LogoutAsync(CancellationToken cancellationToken)
    {
        return objectStorage.IsExistsAsync(StorageIds.LoginId)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return objectStorage.DeleteAsync(StorageIds.LoginId);
                }
                
                return Result.AwaitableFalse;
            }, cancellationToken)
           .IfSuccessAsync(() => navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => cancellationToken.InvokeUIBackgroundAsync(() => mainSplitViewModel.IsPaneOpen = false),
                cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        return navigator.NavigateToAsync(type, cancellationToken)
           .IfSuccessAsync(() => cancellationToken.InvokeUIBackgroundAsync(() => mainSplitViewModel.IsPaneOpen = false),
                cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> BackAsync(CancellationToken cancellationToken)
    {
        return BackCore(cancellationToken).ConfigureAwait(false);
    }
    
    private static async ValueTask<Result> BackCore(CancellationToken cancellationToken)
    {
        var result = await navigator.NavigateBackAsync(cancellationToken);
        
        return new(result.Errors);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SetDueDateTimeAsync(
        IDueDateTimeProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowDateTimeConfirmDialogAsync(
            value => dialogViewer.CloseInputDialogAsync(cancellationToken)
               .IfSuccessAsync(() => cancellationToken.InvokeUIBackgroundAsync(() => property.DueDateTime = value),
                    cancellationToken), calendar =>
            {
                calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime;
                calendar.SelectedTime = TimeSpan.Zero;
            }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SetToDoShortItemAsync(
        IToDoShortItemProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(itemNotify => dialogViewer
           .CloseInputDialogAsync(cancellationToken)
           .IfSuccessAsync(() => cancellationToken.InvokeUIBackgroundAsync(() => property.ShortItem = new()
            {
                Id = itemNotify.Id,
                Name = itemNotify.Name,
            }), cancellationToken), view =>
        {
            if (property.ShortItem is null)
            {
                return;
            }
            
            view.DefaultSelectedItemId = property.ShortItem.Id;
        }, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return toDoService.RemoveFavoriteToDoItemAsync(id, cancellationToken)
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return toDoService.AddFavoriteToDoItemAsync(id, cancellationToken)
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(ILink item, CancellationToken cancellationToken)
    {
        var link = item.Link.ThrowIfNull().ToUri();
        
        return openerLink.OpenLinkAsync(link, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> ChangeOrderIndexAsync(
        ToDoItemNotify item,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(viewModel =>
            {
                var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                
                return dialogViewer.CloseContentDialogAsync(cancellationToken)
                   .IfSuccessAsync(() => toDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken),
                        cancellationToken)
                   .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
            }, _ => dialogViewer.CloseContentDialogAsync(cancellationToken), viewModel => viewModel.Id = item.Id,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        IIdProperty id,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            itemNotify => dialogViewer.CloseInputDialogAsync(cancellationToken)
               .IfSuccessAsync(() => toDoService.CloneToDoItemAsync(id.Id, itemNotify.Id, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            view => view.DefaultSelectedItemId = id.Id, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(
        IDeletable deletable,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(_ => dialogViewer
               .CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(() => toDoService.DeleteToDoItemAsync(deletable.Id, cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() =>
                {
                    if (deletable.IsNavigateToParent)
                    {
                        if (deletable.ParentId is null)
                        {
                            return navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken);
                        }
                        
                        return navigator.NavigateToAsync<ToDoItemViewModel>(
                            viewModel => viewModel.Id = deletable.ParentId.Value, cancellationToken);
                    }
                    
                    return Result.AwaitableFalse;
                }, cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), view => view.ToDoItemId = deletable.Id,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SelectAllAsync(
        AvaloniaList<Selected<ToDoItemNotify>> items,
        CancellationToken cancellationToken
    )
    {
        return cancellationToken.InvokeUIBackgroundAsync(() =>
        {
            if (items.All(x => x.IsSelect))
            {
                foreach (var item in items)
                {
                    item.IsSelect = false;
                }
            }
            else
            {
                foreach (var item in items)
                {
                    item.IsSelect = true;
                }
            }
        });
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SwitchCompleteToDoItemAsync(
        ICanCompleteProperty property,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAsync(() =>
            {
                switch (property.IsCan)
                {
                    case ToDoItemIsCan.None:
                        return Result.AwaitableFalse;
                    case ToDoItemIsCan.CanComplete:
                        return toDoService.UpdateToDoItemCompleteStatusAsync(property.CurrentId, true,
                            cancellationToken);
                    case ToDoItemIsCan.CanIncomplete:
                        return toDoService.UpdateToDoItemCompleteStatusAsync(property.CurrentId, false,
                            cancellationToken);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }, cancellationToken)
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return Result.AwaitableFalse;
        }
        
        return refresh.RefreshAsync(cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> NavigateToToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = id, cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SwitchPaneAsync(CancellationToken cancellationToken)
    {
        return cancellationToken.InvokeUIBackgroundAsync(() =>
            mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen);
    }
    
    private static CommandItem CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, func);
        result.ThrownExceptions.Subscribe(OnNextError);
        
        return result;
    }
    
    private static CommandItem CreateCommand<TParam>(
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, func);
        result.ThrownExceptions.Subscribe(OnNextError);
        
        return result;
    }
    
    private static async void OnNextError(Exception exception)
    {
        await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
    }
}