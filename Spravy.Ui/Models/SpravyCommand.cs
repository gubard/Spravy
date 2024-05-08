namespace Spravy.Ui.Models;

public class SpravyCommand
{
    private static readonly Dictionary<Type, SpravyCommand> createNavigateToCache;
    
    static SpravyCommand()
    {
        createNavigateToCache = new();
    }
    
    private SpravyCommand(TaskWork work, ICommand command)
    {
        Work = work;
        Command = command;
    }
    
    public TaskWork Work { get; }
    public ICommand Command { get; }
    
    public static SpravyCommand CreateNavigateTo<TViewModel>(INavigator navigator, IErrorHandler errorHandler)  where TViewModel : INavigatable
    {
        if (createNavigateToCache.TryGetValue(typeof(TViewModel), out var command))
        {
            return command;
        }
        
        var result = Create(navigator.NavigateToAsync<TViewModel>, errorHandler);
        createNavigateToCache.Add(typeof(TViewModel), result);
        
        return result;
    }
    
    public static SpravyCommand Create(Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func, IErrorHandler errorHandler)
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
    
    public static SpravyCommand Create<TParam>(Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func, IErrorHandler errorHandler)
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