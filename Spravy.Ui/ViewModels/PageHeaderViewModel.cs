using Avalonia.Collections;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PageHeaderViewModel : ViewModelBase
{
    private CommandItem? leftCommand;
    private CommandItem? rightCommand;
    private object? header;

    public CommandItem? LeftCommand
    {
        get => leftCommand;
        set => this.RaiseAndSetIfChanged(ref leftCommand, value);
    }

    public CommandItem? RightCommand
    {
        get => rightCommand;
        set => this.RaiseAndSetIfChanged(ref rightCommand, value);
    }

    public object? Header
    {
        get => header;
        set => this.RaiseAndSetIfChanged(ref header, value);
    }

    public AvaloniaList<CommandItem> Commands { get; } = new();
}