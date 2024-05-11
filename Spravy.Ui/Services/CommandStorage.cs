using Spravy.Ui.Features.ToDo.Interfaces;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    private static readonly INavigator navigator;
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IToDoService toDoService;
    private static readonly IToDoCache toDoCache;
    private static readonly IOpenerLink openerLink;
    private static readonly IMapper mapper;
    private static readonly IAuthenticationService authenticationService;
    private static readonly IObjectStorage objectStorage;
    private static readonly IPasswordService passwordService;
    private static readonly IErrorHandler errorHandler;
    
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        toDoCache = kernel.Get<IToDoCache>();
        passwordService = kernel.Get<IPasswordService>();
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
        
        SelectAll = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(SelectAllAsync, MaterialIconKind.CheckAll,
            "Select all");
        
        OpenLinkItem = CreateCommand<ILink>(OpenLinkAsync, MaterialIconKind.Link, "Open link");
        
        RemoveToDoItemFromFavoriteItem = CreateCommand<Guid>(
            RemoveFavoriteToDoItemAsync, MaterialIconKind.Star, "Remove from favorite");
        
        AddToDoItemToFavoriteItem = CreateCommand<Guid>(
            AddFavoriteToDoItemAsync, MaterialIconKind.StarOutline, "Add to favorite");
        
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
        
        NavigateToCurrentToDoItemItem = CreateCommand(NavigateToCurrentToDoItemAsync, MaterialIconKind.ArrowRight,
            "Open current to-do item");
        
        MultiCompleteToDoItemsItem = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(
            MultiSwitchCompleteToDoItemsAsync, MaterialIconKind.CheckAll, "Complete all to-do items");
        
        MultiSetParentToDoItemsItem = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(MultiSetParentToDoItemsAsync,
            MaterialIconKind.SwapHorizontal, "Set parent for all to-do items");
        
        MultiMoveToDoItemsToRootItem = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(MultiMoveToDoItemsToRootAsync,
            MaterialIconKind.FamilyTree, "Move items to root");
        
        MultiSetTypeToDoItemsItem = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(MultiSetTypeToDoItemsAsync,
            MaterialIconKind.Switch, "Set type all to-do items");
        
        SendNewVerificationCodeItem = CreateCommand<IVerificationEmail>(SendNewVerificationCodeAsync,
            MaterialIconKind.CodeString, "Verification email");
        
        ResetToDoItemItem = CreateCommand<ICurrentIdProperty>(
            ResetToDoItemAsync, MaterialIconKind.EncryptionReset, "Reset to-do item");
        
        MultiDeleteToDoItemsItem = CreateCommand<AvaloniaList<ToDoItemEntityNotify>>(MultiDeleteToDoItemsAsync,
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
        AvaloniaList<ToDoItemEntityNotify> items,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAllAsync(cancellationToken, items.Where(x => x.IsSelected)
               .Select(x => x.Id)
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
        AvaloniaList<ToDoItemEntityNotify> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        return Result.AwaitableFalse
           .IfSuccessAllAsync(cancellationToken, itemsNotify.Where(x => x.IsSelected)
               .Select<ToDoItemEntityNotify, Func<ConfiguredValueTaskAwaitable<Result>>>(x =>
                {
                    var id = x.Id;
                    
                    return () => toDoService.ToDoItemToRootAsync(id, cancellationToken);
                })
               .ToArray())
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> MultiSetTypeToDoItemsAsync(
        AvaloniaList<ToDoItemEntityNotify> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelected).Select(x => x.Id).ToArray();
        
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
        AvaloniaList<ToDoItemEntityNotify> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelected).Select(x => x.Id).ToArray();
        
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
        AvaloniaList<ToDoItemEntityNotify> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var items = itemsNotify.Where(x => x.IsSelected).ToArray();
        
        return CompleteAsync(items, cancellationToken)
           .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> CompleteAsync(
        IEnumerable<ToDoItemEntityNotify> items,
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
           .IfSuccessForEachAsync(id => toDoCache.GetToDoItem(id), cancellationToken)
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
    
    private static ConfiguredValueTaskAwaitable<Result> SelectAllAsync(
        AvaloniaList<ToDoItemEntityNotify> items,
        CancellationToken cancellationToken
    )
    {
        return cancellationToken.InvokeUIBackgroundAsync(() =>
        {
            if (items.All(x => x.IsSelected))
            {
                foreach (var item in items)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in items)
                {
                    item.IsSelected = true;
                }
            }
        });
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