using CommunityToolkit.Mvvm.Input;

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
        Func<CancellationToken, Cvtar> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        var work = TaskWork.Create(errorHandler, ct => taskProgressService.RunProgressAsync(func, ct));

        var command = new AsyncRelayCommand(
            async () =>
            {
                try
                {
                    await work.RunAsync();
                }
                catch (Exception e)
                {
                    await errorHandler.ExceptionHandleAsync(e, CancellationToken.None);
                }
            }
        );

        return new(work, command);
    }

    public static SpravyCommand Create<TParam>(
        Func<TParam, CancellationToken, Cvtar> func,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        var work = TaskWork.Create<TParam>(
            errorHandler,
            (param, ct) => taskProgressService.RunProgressAsync(func, param, ct)
        );

        var command = new AsyncRelayCommand<TParam>(
            async p =>
            {
                try
                {
                    await work.RunAsync(p.ThrowIfNull());
                }
                catch (Exception e)
                {
                    await errorHandler.ExceptionHandleAsync(e, CancellationToken.None);
                }
            }
        );

        return new(work, command);
    }
}