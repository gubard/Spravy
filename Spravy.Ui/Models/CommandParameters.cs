using Spravy.Domain.Models;

namespace Spravy.Ui.Models;

public readonly struct CommandParameters
{
    public CommandParameters(ToDoItemCommand command, TaskWork work)
    {
        Command = command;
        Work = work;
    }

    public ToDoItemCommand Command { get; }
    public TaskWork Work { get; }
}

public readonly struct CommandParameters<T>
{
    public CommandParameters(ToDoItemCommand command, TaskWork<T> work)
    {
        Command = command;
        Work = work;
    }

    public ToDoItemCommand Command { get; }
    public TaskWork<T> Work { get; }
}