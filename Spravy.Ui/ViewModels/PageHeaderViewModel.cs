namespace Spravy.Ui.ViewModels;

public class PageHeaderViewModel : ViewModelBase
{
    [Reactive]
    public CommandItem? LeftCommand { get; set; }

    [Reactive]
    public CommandItem? RightCommand { get; set; }

    [Reactive]
    public object? Header { get; set; }

    public AvaloniaList<CommandItem> Commands { get; } = new();
}