using Spravy.Core.Helpers;
using Spravy.Ui.Mappers;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IPasswordService passwordService;
    private static readonly IErrorHandler errorHandler;
    
    static CommandStorage()
    {
        var kernel = DiHelper.ServiceFactory.ThrowIfNull();
        passwordService = kernel.CreateService<IPasswordService>();
        dialogViewer = kernel.CreateService<IDialogViewer>();
        mainSplitViewModel = kernel.CreateService<MainSplitViewModel>();
        errorHandler = kernel.CreateService<IErrorHandler>();
        
        AddPasswordItemItem = CreateCommand(AddPasswordItemAsync, MaterialIconKind.Plus, "Add password item");
        
        ShowPasswordItemSettingItem = CreateCommand<IIdProperty>(ShowPasswordItemSettingAsync,
            MaterialIconKind.Settings, "Show password setting");
    }
    
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