using Spravy.Domain.Models;

namespace Spravy.Ui.Models;

public readonly struct CommandParameters
{
    public CommandParameters(CommandItem value, TaskWork work)
    {
        Value = value;
        Work = work;
    }

    public CommandItem Value { get; }
    public TaskWork Work { get; }
}

public readonly struct CommandParameters<T>
{
    public CommandParameters(CommandItem value, TaskWork<T> work)
    {
        Value = value;
        Work = work;
    }

    public CommandItem Value { get; }
    public TaskWork<T> Work { get; }
}