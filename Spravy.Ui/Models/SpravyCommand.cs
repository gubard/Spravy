namespace Spravy.Ui.Models;

public class SpravyCommand
{
    private SpravyCommand(TaskWork work, ICommand command)
    {
        Work = work;
        Command = command;
    }

    public TaskWork Work { get; }
    public ICommand Command { get; }

    public static SpravyCommand Create(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }

        var work = TaskWork.Create(
            errorHandler,
            ct => taskProgressService.RunProgressAsync(func, ct)
        );
        var command = ReactiveCommand.CreateFromTask(() => work.RunAsync());
        command.ThrownExceptions.Subscribe(OnNextError);

        return new(work, command);
    }

    public static SpravyCommand Create<TParam>(
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        async void OnNextError(Exception exception)
        {
            await errorHandler.ExceptionHandleAsync(exception, CancellationToken.None);
        }

        var work = TaskWork.Create<TParam>(
            errorHandler,
            (param, ct) => taskProgressService.RunProgressAsync(func, param, ct)
        );
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync<TParam>);
        command.ThrownExceptions.Subscribe(OnNextError);

        return new(work, command);
    }
}
