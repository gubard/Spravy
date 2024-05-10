using Spravy.Ui.Errors;

namespace Spravy.Ui.Models;

public class SpravyCommand
{
    private static readonly Dictionary<Type, SpravyCommand> createNavigateToCache;
    private static readonly Dictionary<Guid, SpravyCommand> deletePasswordItemCache;
    private static SpravyCommand? _sendNewVerificationCode;
    private static SpravyCommand? _back;
    
    static SpravyCommand()
    {
        createNavigateToCache = new();
        deletePasswordItemCache = new();
    }
    
    private SpravyCommand(TaskWork work, ICommand command)
    {
        Work = work;
        Command = command;
    }
    
    public TaskWork Work { get; }
    public ICommand Command { get; }
    
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