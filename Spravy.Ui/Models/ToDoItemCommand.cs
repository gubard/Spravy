using System.Windows.Input;
using Material.Icons;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class ToDoItemCommand : NotifyBase
{
    private MaterialIconKind icon;
    private ICommand command;
    private string name;

    public ToDoItemCommand(MaterialIconKind icon, ICommand command, string name)
    {
        this.icon = icon;
        this.command = command;
        this.name = name;
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

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
}