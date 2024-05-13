using Spravy.Ui.Errors;
using Spravy.Ui.Features.PasswordGenerator.Interfaces;

namespace Spravy.Ui.Models;

public class SpravyCommand
{
    private static readonly Dictionary<Type, SpravyCommand> createNavigateToCache;
    private static readonly Dictionary<Guid, SpravyCommand> deletePasswordItemCache;
    private static readonly Dictionary<Guid, SpravyCommand> generatePasswordCache;
    private static SpravyCommand? _sendNewVerificationCode;
    private static SpravyCommand? _back;
    private static SpravyCommand? _navigateToCurrentToDoItem;
    
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
    
    public static SpravyCommand CreateNavigateToCurrentToDoItem(
        IToDoService toDoService,
        INavigator navigator,
        ITaskProgressService taskProgressService,
        IErrorHandler errorHandler
    )
    {
        if (_navigateToCurrentToDoItem is not null)
        {
            return _navigateToCurrentToDoItem;
        }
        
        ConfiguredValueTaskAwaitable<Result> NavigateToCurrentToDoItemAsync(CancellationToken cancellationToken)
        {
            return taskProgressService.RunProgressAsync(() => toDoService
               .GetCurrentActiveToDoItemAsync(cancellationToken)
               .IfSuccessAsync(activeToDoItem =>
                {
                    if (activeToDoItem.HasValue)
                    {
                        return navigator.NavigateToAsync<ToDoItemViewModel>(
                            viewModel => viewModel.Id = activeToDoItem.Value.Id, cancellationToken);
                    }
                    
                    return navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken);
                }, cancellationToken), cancellationToken);
        }
        
        _navigateToCurrentToDoItem = Create(NavigateToCurrentToDoItemAsync, errorHandler);
        
        return _navigateToCurrentToDoItem;
    }
    
    public static SpravyCommand CreateGeneratePassword(
        IPasswordItem passwordItem,
        IPasswordService passwordService,
        IClipboardService clipboard,
        ISpravyNotificationManager spravyNotificationManager,
        IErrorHandler errorHandler
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
        
        var result = Create(GeneratePasswordAsync, errorHandler);
        generatePasswordCache.Add(passwordItem.Id, result);
        
        return result;
    }
    
    public static SpravyCommand CreateDeletePasswordItem(
        Guid id,
        IPasswordService passwordService,
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler
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
        
        var result = Create(DeletePasswordItemAsync, errorHandler);
        deletePasswordItemCache.Add(id, result);
        
        return result;
    }
    
    public static SpravyCommand CreateBack(INavigator navigator, IErrorHandler errorHandler)
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
        
        _back = Create(c => BackCore(c).ConfigureAwait(false), errorHandler);
        
        return _back;
    }
    
    public static SpravyCommand CreateSendNewVerificationCode(
        IAuthenticationService authenticationService,
        IErrorHandler errorHandler
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
        
        _sendNewVerificationCode = Create<IVerificationEmail>(SendNewVerificationCodeAsync, errorHandler);
        
        return _sendNewVerificationCode;
    }
    
    public static SpravyCommand CreateNavigateTo<TViewModel>(INavigator navigator, IErrorHandler errorHandler)
        where TViewModel : INavigatable
    {
        if (createNavigateToCache.TryGetValue(typeof(TViewModel), out var command))
        {
            return command;
        }
        
        var result = Create(navigator.NavigateToAsync<TViewModel>, errorHandler);
        createNavigateToCache.Add(typeof(TViewModel), result);
        
        return result;
    }
    
    public static SpravyCommand Create(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }
        
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(() => work.RunAsync());
        command.ThrownExceptions.Subscribe(OnNextError);
        
        return new(work, command);
    }
    
    public static SpravyCommand Create<TParam>(
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }
        
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync<TParam>);
        command.ThrownExceptions.Subscribe(OnNextError);
        
        return new(work, command);
    }
}