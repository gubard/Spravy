using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Material.Icons;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Models;

public class CommandItem
{
    private CommandItem(
        MaterialIconKind icon,
        ICommand command,
        string name,
        TaskWork work,
        IObservable<Exception> thrownExceptions
    )
    {
        Icon = icon;
        Command = command;
        Name = name;
        Work = work;
        ThrownExceptions = thrownExceptions;
    }

    private CommandItem(
        MaterialIconKind icon,
        ICommand command,
        string name,
        object? parameter,
        TaskWork work,
        IObservable<Exception> thrownExceptions
    )
    {
        Icon = icon;
        Command = command;
        Name = name;
        Parameter = parameter;
        Work = work;
        ThrownExceptions = thrownExceptions;
    }

    public MaterialIconKind Icon { get; }
    public TaskWork Work { get; }
    public ICommand Command { get; }
    public string Name { get; }
    public object? Parameter { get; }
    public IObservable<Exception> ThrownExceptions { get; }

    public static CommandItem Create(
        MaterialIconKind icon,
        string name,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(() => work.RunAsync());

        return new(icon, command, name, work, command.ThrownExceptions);
    }

    public static CommandItem Create<TParam>(
        MaterialIconKind icon,
        string name,
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync<TParam>);

        return new(icon, command, name, work, command.ThrownExceptions);
    }

    public static CommandItem Create(
        MaterialIconKind icon,
        string name,
        object parameter,
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(() => work.RunAsync());

        return new(icon, command, name, parameter, work, command.ThrownExceptions);
    }

    public static CommandItem Create<TParam>(
        MaterialIconKind icon,
        string name,
        object parameter,
        Func<TParam, CancellationToken, ConfiguredValueTaskAwaitable<Result>> func
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync<TParam>);

        return new(icon, command, name, parameter, work, command.ThrownExceptions);
    }

    public CommandItem WithParam(object parameter)
    {
        return new(Icon, Command, Name, parameter, Work, ThrownExceptions);
    }
}