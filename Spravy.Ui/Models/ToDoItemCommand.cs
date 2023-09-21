using System.Windows.Input;
using Material.Icons;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoItemCommand : NotifyBase
{
    private MaterialIconKind icon;
    private ICommand command;

    public ToDoItemCommand(MaterialIconKind icon, ICommand command)
    {
        Icon = icon;
        this.command = command;
    }

    public MaterialIconKind Icon
    {
        get => icon;
        set => this.RaiseAndSetIfChanged(ref icon, value);
    }

    public ICommand Command
    {
        get => command;
        set => this.RaiseAndSetIfChanged(ref command, value);
    }
}