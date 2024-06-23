using Spravy.Core.Helpers;
using Spravy.Ui.Mappers;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    private static readonly INavigator navigator;
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IToDoService toDoService;
    private static readonly IToDoCache toDoCache;
    private static readonly IObjectStorage objectStorage;
    private static readonly IPasswordService passwordService;
    private static readonly IErrorHandler errorHandler;
    
    static CommandStorage()
    {
        var kernel = DiHelper.ServiceFactory.ThrowIfNull();
        toDoCache = kernel.CreateService<IToDoCache>();
        passwordService = kernel.CreateService<IPasswordService>();
        objectStorage = kernel.CreateService<IObjectStorage>();
        navigator = kernel.CreateService<INavigator>();
        dialogViewer = kernel.CreateService<IDialogViewer>();
        mainSplitViewModel = kernel.CreateService<MainSplitViewModel>();
        toDoService = kernel.CreateService<IToDoService>();
        errorHandler = kernel.CreateService<IErrorHandler>();
        LogoutItem = CreateCommand(LogoutAsync, MaterialIconKind.Logout, "Logout");
        AddRootToDoItemItem = CreateCommand(AddRootToDoItemAsync, MaterialIconKind.Plus, "Add root to-do item");
        
        ToDoItemSearchItem = CreateCommand<IToDoItemSearchProperties>(
            ToDoItemSearchAsync, MaterialIconKind.Search, "Search to-do item");
        
        SetToDoDescriptionItem = CreateCommand<ToDoItemEntityNotify>(SetToDoDescriptionAsync, MaterialIconKind.Pencil,
            "Set to-do item description");
        
        AddPasswordItemItem = CreateCommand(AddPasswordItemAsync, MaterialIconKind.Plus, "Add password item");
        
        ShowPasswordItemSettingItem = CreateCommand<IIdProperty>(ShowPasswordItemSettingAsync,
            MaterialIconKind.Settings, "Show password setting");
    }
    
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
                    () => passwordService.AddPasswordItemAsync(vm.ToAddPasswordOptions(), cancellationToken),
                    cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken), ActionHelper<AddPasswordItemViewModel>.Empty,
            cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> SetToDoDescriptionAsync(
        ToDoItemEntityNotify property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
            viewModel => dialogViewer.CloseContentDialogAsync(cancellationToken)
               .IfSuccessAsync(
                    () => toDoService.UpdateToDoItemDescriptionAsync(property.Id, viewModel.Content.Description,
                        cancellationToken), cancellationToken)
               .IfSuccessAsync(
                    () => toDoService.UpdateToDoItemDescriptionTypeAsync(property.Id, viewModel.Content.Type,
                        cancellationToken), cancellationToken)
               .IfSuccessAsync(() => RefreshCurrentViewAsync(cancellationToken), cancellationToken),
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
                ids => properties.ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), false, cancellationToken),
                cancellationToken);
    }
    
    private static ConfiguredValueTaskAwaitable<Result> AddRootToDoItemAsync(CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(view =>
            {
                var options = view.ToAddRootToDoItemOptions();
                
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
                
                return Result.AwaitableSuccess;
            }, cancellationToken)
           .IfSuccessAsync(() => navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken),
                cancellationToken)
           .IfSuccessAsync(() => cancellationToken.InvokeUiBackgroundAsync(() =>
            {
                mainSplitViewModel.IsPaneOpen = false;
                
                return Result.Success;
            }), cancellationToken);
    }
    
    private static async ValueTask<Result> BackCore(CancellationToken cancellationToken)
    {
        var result = await navigator.NavigateBackAsync(cancellationToken);
        
        return new(result.Errors);
    }
    
    public static ConfiguredValueTaskAwaitable<Result> RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return Result.AwaitableSuccess;
        }
        
        return refresh.RefreshAsync(cancellationToken);
    }
    
    private static CommandItem CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, errorHandler, func);
        result.ThrownExceptions.Subscribe(OnNextError);
        
        return result;
    }
    
    private static CommandItem CreateCommand<TParam>(
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, errorHandler, func);
        result.ThrownExceptions.Subscribe(OnNextError);
        
        return result;
    }
    
    private static async void OnNextError(Exception exception)
    {
        await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
    }
}